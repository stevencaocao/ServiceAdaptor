namespace AuthManager.ApiResult
{
    /// <summary>
    /// 自定义异常
    /// </summary>
    public class FirstArgsException : FirstException
    {
        /// <summary>
        /// 不定参数
        /// </summary>
        public object[] args;

        /// <summary>
        /// 自定异常
        /// </summary>
        /// <param name="message">错误消息</param>
        /// <param name="args">占位符参数</param>
        public FirstArgsException(string message, params object[] args)
        {
            code = ErrorCode.UNKNOWN_ERROR;
            this.args = args;
            msg = message;
        }

        /// <summary>
        /// 自定异常
        /// </summary>
        /// <param name="code">错误码</param>
        /// <param name="args">占位符参数</param>
        public FirstArgsException(ErrorCode code, params object[] args)
        {
            this.code = code;
            this.args = args;
        }

        /// <summary>
        /// 获取异常消息
        /// </summary>
        /// <returns></returns>
        public override string? getMessage()
        {
            string? errorInfo = msg;
            if (args != null && args.Length > 0)
            {
                for (int i = 0, len = args.Length; i < len; i++)
                {
                    string? arg = Convert.ToString(args[i]);
                    errorInfo = errorInfo?.Replace("{" + i + "}", arg);
                }
            }
            return code == ErrorCode.UNKNOWN_ERROR ? errorInfo : code.getInfo(args);
        }
    }
}
