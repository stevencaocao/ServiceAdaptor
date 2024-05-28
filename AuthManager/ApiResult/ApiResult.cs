using Newtonsoft.Json;

namespace AuthManager.ApiResult
{
    /// <summary>
    /// 接口统一返回值
    /// </summary>
    public class ApiResult<T>
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int Code { get; set; }
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool? Success { get; set; }
        /// <summary>
        /// 消息内容
        /// </summary>
        public string? Msg { get; set; }
        /// <summary>
        /// 返回值
        /// </summary>
        public T? Obj { get; set; }

        /// <summary>
        /// 返回结果
        /// </summary>
        /// <param name="obj">结果</param>
        public ApiResult(T? obj)
        {
            Success = true;
            Obj = obj;
            Msg = ErrorCode.SUCCESS.getInfo();
            Code = ErrorCode.SUCCESS.getCodeInt();
        }

        /// <summary>
        /// 返回异常结果
        /// </summary>
        /// <param name="ex">异常信息</param>
        public ApiResult(Exception ex)
        {
            if (ErrorInfoUtil.isContain(ex.Message, out ErrorCode errorCode))
            {
                Success = false;
                Msg = errorCode.getInfo();
                Code = errorCode.getCodeInt();
            }
            else
            {
                Success = false;
                Obj = (T)(object)ex.Message;
                Msg = ErrorCode.ERROR.getInfo();
                Code = ErrorCode.ERROR.getCodeInt();
            }
        }
        /// <summary>
        /// 返回自定义异常结果
        /// </summary>
        /// <param name="ex">自定义异常</param>
        public ApiResult(FirstException ex)
        {
            Success = false;
            Msg = ex.getMessage();
            Code = ex.getCodeInt();
        }
        /// <summary>
        /// 返回带参数的异常结果
        /// </summary>
        /// <param name="ex">自定义异常</param>
        public ApiResult(FirstArgsException ex)
        {
            Success = false;
            Msg = ex.getMessage();
            Code = ex.getCodeInt();
        }
        /// <summary>
        /// 返回成功的结果
        /// </summary>
        /// <param name="obj">结果</param>
        /// <returns></returns>
        public ApiResult<T> success(T obj)
        {
            Success = true;
            Obj = obj;
            Msg = ErrorCode.SUCCESS.getInfo();
            Code = ErrorCode.SUCCESS.getCodeInt();
            return this;
        }
        /// <summary>
        /// 返回错误的结果
        /// </summary>
        /// <param name="msg">消息</param>
        /// <returns></returns>
        public ApiResult<T> error(string msg)
        {
            Success = false;
            Msg = msg;
            Code = ErrorCode.ERROR.getCodeInt();
            return this;
        }
        /// <summary>
        /// 返回默认的错误
        /// </summary>
        /// <returns></returns>
        public ApiResult<T> error()
        {
            Success = false;
            Msg = ErrorCode.ERROR.getInfo();
            Code = ErrorCode.ERROR.getCodeInt();
            return this;
        }

        /// <summary>
        /// 转json
        /// </summary>
        /// <returns></returns>
        public string toJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
