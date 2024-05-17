using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ServiceAdapter.Common;
using Swashbuckle.AspNetCore.Filters;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapter.JwtToken
{
    public static class IWebHostBuilderExtensions_UseJwtToken
    {
        /// <summary>
        /// 使用jwt授权，鉴权
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="consulConfig"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseJwtToken(this IWebHostBuilder builder, IConfiguration configuration)
        {
            builder.ConfigureServices(delegate (IServiceCollection services)
            {
                services.AddSingleton<IJwtTokenUtils, JwtTokenUtils>();

                services.AddSwaggerGen(options =>
                {
                    // 在header中添加token，传递到后台
                    options.OperationFilter<SecurityRequirementsOperationFilter>();

                    //region Token绑定到ConfigureServices，swagger右上角显示Token输入框
                    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Description = "JWT授权(数据将在请求头中进行传输) 直接在下框中输入Bearer {token}（注意两者之间是一个空格）\"",
                        Name = "Authorization",//jwt默认的参数名称
                        In = ParameterLocation.Header,//jwt默认存放Authorization信息的位置(请求头中)
                        Type = SecuritySchemeType.ApiKey
                    });
                });


                var jwt = configuration.GetSection("JWT");
                var algorithmsType = jwt["AlgorithmsType"];

                if (algorithmsType == "RsaSha256")
                {
                    string keyDir = Directory.GetCurrentDirectory();
                    if (RSAHelper.TryGetKeyParameters(keyDir, true, out SecurityKey securityKey) == false)
                    {
                        securityKey = RSAHelper.GenerateAndSaveKey(keyDir);
                    }
                }

                services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
                {
                    var jwtSecurityKey = jwt["JwtSecurityKey"];
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(string.IsNullOrEmpty(jwtSecurityKey) ? "123456" : jwtSecurityKey));

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,//是否验证Issuer
                        ValidateAudience = true,//是否验证Audience
                        ValidateLifetime = true,//是否验证失效时间---默认还添加了300s后才过期
                        ValidateIssuerSigningKey = true,//是否验证SecurityKey
                        ValidAudience = jwt["ValidAudience"], //Audience,需要跟前面签发jwt的设置一致
                        ValidIssuer = jwt["ValidIssuer"], //Issuer，这两项和前面签发jwt的设置一致
                        ClockSkew = TimeSpan.FromMinutes(Convert.ToInt32(jwt["ClockSkew"])),//token过期后立马过期
                        IssuerSigningKey = algorithmsType == "RsaSha256" ? RSAHelper.GetPublicKey(jwt["publicKey"]) : key,//拿到SecurityKey
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnChallenge = async context =>
                        {
                            context.HandleResponse();
                            context.Response.ContentType = "application/json;charset=utf-8";
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await context.Response.WriteAsync("{\"message\":\"Unauthorized\",\"success\":false}");
                        },
                        OnAuthenticationFailed = async context =>
                        {
                            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                            {
                                context.Response.Headers.Add("tokenExpired", "true");//延迟时间也到了的过期，刷新token
                            }
                            await Task.CompletedTask;
                        }
                    };
                });
            });


            return builder;
        }
    }
}
