using ServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapter
{
    public interface IApiClient
    {
        /// <summary>
        /// 同步请求
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        ApiResponse<ReturnType> RequestApi<ReturnType>(ApiRequest req);

        /// <summary>
        /// 异步请求
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        Task<ApiResponse<ReturnType>> RequestApiAsync<ReturnType>(ApiRequest req);
    }
}
