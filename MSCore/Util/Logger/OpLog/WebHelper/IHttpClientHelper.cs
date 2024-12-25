using System.Collections.Generic;
using System.Threading.Tasks;

namespace MSCore.Util.Logger.OpLog.WebHelper
{
    public interface IHttpClientHelper
    {
        /// <summary>
        /// 通用post方法
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="data">请求参数</param>
        /// <returns></returns>
        Task<ApiResponse<T>> Post<T>(string url, string data, Dictionary<string, string> header = null);


        /// <summary>
        /// 通用get方法
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="args">请求参数</param>
        /// <returns></returns>
        Task<ApiResponse<T>> Get<T>(string url, object args, Dictionary<string, string> header = null);
    }
}
