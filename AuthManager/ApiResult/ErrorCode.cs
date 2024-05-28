using System.ComponentModel;

namespace AuthManager.ApiResult
{
    public enum ErrorCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        [Description("成功！")]
        SUCCESS = 200,

        /// <summary>
        /// 常规错误
        /// </summary>
        [Description("常规错误")]
        ERROR = 300,

        /// <summary>
        /// 账号不存在
        /// </summary>
        [Description("账号不存在")]
        ACCOUNT_NOT_EXISTS = 4000,

        /// <summary>
        /// 未知错误
        /// </summary>
        [Description("未知错误")]
        UNKNOWN_ERROR = 500,
        /// <summary>
        /// 未经授权
        /// </summary>
        [Description("未经授权")]
        UNAUTHORIZED = 401
    }
}
