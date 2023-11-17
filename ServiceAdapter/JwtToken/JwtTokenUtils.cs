using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ServiceAdapter.Common;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ServiceAdapter.JwtToken
{
    public class JwtTokenUtils : IJwtTokenUtils
    {
        private readonly IConfiguration _configuration;
        public JwtTokenUtils(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="t">用户信息(不要携带敏感信息，需要携带userCode字段作为默认唯一识别码，如需使用其他字段，请配置第三个参数)</param>
        /// <param name="validateToken">用户生成refreshToken的原验证Token</param>
        /// <param name="userCode">用户唯一识别码默认为userCode，如需其他字段作为唯一识别码可修改</param>
        /// <returns></returns>
        public Object getToken<T>(T t, SecurityToken validateToken = null, string userCode = "userCode")
        {
            var jwtToken = validateToken == null ? null : validateToken as JwtSecurityToken;//转换一下
            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var authTime = DateTime.Now;
            var expires = _configuration["JWT:Expires"];
            var jwtSecurityKey = _configuration["JWT:JwtSecurityKey"];
            var algorithmsType = _configuration["JWT:AlgorithmsType"];
            var issuer = _configuration["JWT:ValidIssuer"];
            var audience = _configuration["JWT:ValidAudience"];
            var refreshExpires = _configuration["JWT:Refresh_Expires"];

            var claims = jwtToken == null ? new[] { new Claim("role", "user") } : jwtToken.Claims;

            DateTime expir = authTime.AddMinutes(Convert.ToInt32(expires));
            var view = new System.Dynamic.ExpandoObject();

            System.Reflection.PropertyInfo[] properties = t.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            foreach (var property in properties)
            {
                if (jwtToken != null)
                {
                    ((IDictionary<string, object>)view).Add(property.Name, "");
                }
                else
                {
                    ((IDictionary<string, object>)view).Add(property.Name, property.GetValue(t, null));
                    claims = claims.Append(new Claim(property.Name, property.GetValue(t, null).ToString()));
                }
            }
            ((IDictionary<string, object>)view).Add("Expires", expir);

            #region 生成token

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(string.IsNullOrEmpty(jwtSecurityKey) ? "123456" : jwtSecurityKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            if (algorithmsType == "RsaSha256")
            {
                string keyDir = Directory.GetCurrentDirectory();
                if (RSAHelper.TryGetKeyParameters(keyDir, true, out SecurityKey securityKey) == false)
                {
                    securityKey = RSAHelper.GenerateAndSaveKey(keyDir);
                }
                credentials = new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256);
            }


            var token = new JwtSecurityToken(issuer:
                issuer, audience: audience, claims: claims, notBefore: authTime, expires: expir, signingCredentials: credentials);
            ((IDictionary<string, object>)view).Add("Token", jwtSecurityTokenHandler.WriteToken(token));

            #endregion

            #region 生成refreshToken

            ((IDictionary<string, object>)view).TryGetValue(userCode, out object code);
            var refClaims = new[]
               {
                    new Claim("role","refresh"),new Claim(userCode,Convert.ToString(code))
                };
            var refreshToken = new JwtSecurityToken(
                issuer: issuer,// 发布者
                audience: audience,// 接收者
                notBefore: authTime,// token签发时间
                expires: authTime.AddDays(Convert.ToInt32(refreshExpires)),// token过期时间
                claims: refClaims,// 该token内存储的自定义字段信息
                signingCredentials: credentials// 用于签发token的秘钥算法
            );
            ((IDictionary<string, object>)view).Add("RefreshToken", jwtSecurityTokenHandler.WriteToken(refreshToken));

            #endregion

            return view;
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="refreshModel">访问令牌参数</param>
        /// <param name="t">用户信息</param>
        /// <returns></returns>
        public Object refresh<T>(RefreshModel refreshModel, T t)
        {
            var algorithmsType = _configuration["JWT:AlgorithmsType"];
            var securityKey = _configuration["JWT:JwtSecurityKey"];
            var issuer = _configuration["JWT:ValidIssuer"];
            var audience = _configuration["JWT:ValidAudience"];
            var publicKey = _configuration["JWT:publicKey"];


            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();

            bool isCan = jwtSecurityTokenHandler.CanReadToken(refreshModel.AccessToken);//验证Token格式
            if (!isCan)
                return new { success = false, message = "传入访问令牌格式错误", data = "" };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(string.IsNullOrEmpty(securityKey) ? "123456" : securityKey));

            var validateParameter = new TokenValidationParameters()//验证参数
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuer,
                ValidAudience = audience,
                IssuerSigningKey = algorithmsType == "RsaSha256" ? RSAHelper.GetPublicKey(publicKey) : key
            };

            //验证传入的过期的AccessToken
            SecurityToken validatedToken = null;
            try
            {
                jwtSecurityTokenHandler.ValidateToken(refreshModel.AccessToken, validateParameter, out validatedToken);//微软提供的验证方法。那个out传出的参数，类型是是个抽象类，记得转换
            }
            catch (SecurityTokenException)
            {
                return new { success = false, message = "传入AccessToken被修改", data = "" };
            }

            var view = getToken(t, validatedToken);
            return new { success = true, message = "ok", data = view };
        }
    }
}
