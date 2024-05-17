using Microsoft.IdentityModel.Tokens;
using System;

namespace ServiceAdapter.JwtToken
{
    public interface IJwtTokenUtils
    {
        /// <summary>
        /// 生成token
        /// </summary>
        /// <param name="t">用户信息(不要携带敏感信息，需要携带userCode字段作为默认唯一识别码，如需使用其他字段，请配置第三个参数)</param>
        /// <param name="validateToken">用户生成refreshToken的原验证Token</param>
        /// <param name="keyCode">用户唯一识别码默认为userCode，如需其他字段作为唯一识别码可修改</param>
        /// <returns></returns>
        Object getToken<T>(T t, SecurityToken validateToken = null,string keyCode = "userCode");

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <param name="refreshModel">访问令牌参数</param>
        /// <param name="t">用户信息</param>
        /// <returns></returns>
        Object refresh<T>(RefreshModel refreshModel,T t);
    }
}
