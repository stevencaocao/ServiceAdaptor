using ServiceAdaptor;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceAdapter
{
    public class ApiClient
    {

        public static IApiClient Instance { get; internal set; }


        #region RequestApi

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ReturnType"></typeparam>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        /// <param name="headers"></param>
        /// <returns></returns>
        public static async Task<ReturnType> RequestApiAsync<ReturnType>(string route, Object arg, string httpMethod = null, IDictionary<string, string> headers = null)
        {
            var response = await Instance.RequestApiAsync<ReturnType>(new ApiRequest { url = route, arg = arg, httpMethod = httpMethod, headers = headers });
            if (response == null)
            {
                return default;
            }
            return response.data;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="route"></param>
        /// <param name="arg"></param>
        /// <param name="httpMethod">可为 GET、POST、DELETE、PUT等,可不指定</param>
        /// <returns></returns>
        public static ReturnType RequestApi<ReturnType>(string route, Object arg, string httpMethod = null, IDictionary<string, string> headers = null)
        {
            var response = Instance.RequestApi<ReturnType>(new ApiRequest { url = route, arg = arg, httpMethod = httpMethod, headers = headers });
            if (response == null)
            {
                return default;
            }
            return response.data;
        }

        #endregion

        public const string Get = "GET";
        public const string Post = "POST";
        public const string Delete = "DELETE";
        public const string Put = "PUT";
        public const string Patch = "PATCH";
    }
}
