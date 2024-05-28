namespace AuthManager.ApiResult
{
    /// <summary>
    /// 自定义异常
    /// </summary>
    public class FirstException : Exception
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public ErrorCode code;
        /// <summary>
        /// 错误消息
        /// </summary>
        public string? msg;
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public FirstException()
        {
            code = ErrorCode.ERROR;
            msg = ErrorCode.ERROR.getInfo();
        }
        /// <summary>
        /// 自定错误消息
        /// </summary>
        /// <param name="message"></param>
        public FirstException(string message)
        {
            code = ErrorCode.UNKNOWN_ERROR;
            msg = message;
        }

        /// <summary>
        /// 自定异常
        /// </summary>
        /// <param name="code">错误码</param>
        public FirstException(ErrorCode code)
        {
            this.code = code;
        }

        /// <summary>
        /// 获取异常消息
        /// </summary>
        /// <returns></returns>
        public virtual string? getMessage()
        {
            return code == ErrorCode.UNKNOWN_ERROR ? msg : code.getInfo();
        }

        /// <summary>
        /// 获取异常错误码字符串
        /// </summary>
        /// <returns></returns>
        public string getCode()
        {
            return code.ToString().ToLower();
        }
        /// <summary>
        /// 获取异常错误码
        /// </summary>
        /// <returns></returns>
        public int getCodeInt()
        {
            return (int)Enum.Parse(typeof(ErrorCode), code.ToString());
        }
    }
}
