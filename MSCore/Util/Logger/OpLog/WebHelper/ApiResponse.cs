using System.Collections.Generic;

namespace MSCore.Util.Logger.OpLog.WebHelper
{
    public class ApiResponse<T>
    {
        public T data;

        public int StatusCode = 200;

        public IDictionary<string, string> headers;

        public string message;

        /// <summary>
        /// 返回结果:
        ///     A value that indicates if the HTTP response was successful. true if System.Net.Http.HttpResponseMessage.StatusCode
        ///     was in the range 200-299; otherwise false.
        /// </summary>
        public bool IsSuccessStatusCode => StatusCode >= 200 && StatusCode <= 299;
    }
}
