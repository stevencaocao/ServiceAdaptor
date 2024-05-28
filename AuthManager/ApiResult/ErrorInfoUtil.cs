using System.ComponentModel;

namespace AuthManager.ApiResult
{
    /// <summary>
    /// 获取错误信息类
    /// </summary>
    public static class ErrorInfoUtil
    {
        /// <summary>
        /// 获取错误码字符串
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string getCode(this Enum val)
        {
            return val.ToString().ToLower();
        }
        /// <summary>
        /// 获取成功还是失败
        /// </summary>
        /// <returns></returns>
        public static bool getState(ErrorCode errorCode)
        {
            return errorCode == ErrorCode.SUCCESS;
        }
        /// <summary>
        /// 获取错误码
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static int getCodeInt(this Enum val)
        {
            return (int)Enum.Parse(typeof(ErrorCode), val.ToString());
        }

        /// <summary>
        /// 根据枚举类型提取错误信息
        /// </summary>
        /// <param name="val">枚举</param>
        /// <returns>错误信息</returns>
        public static string getInfo(this Enum val)
        {
            var field = val.GetType().GetField(val.ToString());
            var customAttribute = Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));
            return customAttribute == null ? val.ToString() : ((DescriptionAttribute)customAttribute).Description;
        }

        /// <summary>
        /// 根据枚举类型提取错误信息，并替换占位符数据
        /// </summary>
        /// <param name="val">枚举</param>
        /// <param name="o">替换占位符数据</param>
        /// <returns>错误信息</returns>
        public static string getInfo(this Enum val, object o)
        {
            return val.getInfo().Replace("{0}", Convert.ToString(o));
        }

        /// <summary>
        /// 根据枚举类型提取错误信息，并替换占位符数据
        /// </summary>
        /// <param name="val">枚举</param>
        /// <param name="o1">替换占位符数据</param>
        /// <param name="o2">替换占位符数据</param>
        /// <returns>错误信息</returns>
        public static string getInfo(this Enum val, object o1, object o2)
        {
            return val.getInfo(o1).Replace("{1}", Convert.ToString(o2));
        }

        /// <summary>
        /// 根据枚举类型提取错误信息，并替换占位符数据。不定长度参数
        /// </summary>
        /// <param name="val">枚举</param>
        /// <param name="args">不定长度替换占位符数据</param>
        /// <returns>错误信息</returns>
        public static string getInfo(this Enum val, params object[] args)
        {
            string errorInfo = val.getInfo();
            if (args != null && args.Length > 0)
            {
                for (int i = 0, len = args.Length; i < len; i++)
                {
                    string arg = Convert.ToString(args[i]);
                    errorInfo = errorInfo.Replace("{" + i + "}", arg);
                }
            }
            return errorInfo;
        }

        /// <summary>
        /// 是否存在枚举类型
        /// </summary>
        /// <param name="code">字符串</param>
        /// <param name="errorCode">返回枚举</param>
        /// <returns></returns>
        public static bool isContain(string code, out ErrorCode errorCode)
        {
            errorCode = ErrorCode.UNKNOWN_ERROR;
            if (string.IsNullOrEmpty(code))
                return false;
            return Enum.TryParse(code, out errorCode);
        }
    }
}
