using System;
using System.Collections.Generic;

namespace ServiceAdaptor
{
    public class ApiRequest
    {
        /// <summary>
        /// 为相对路径，如 "/api/Values/get"
        /// </summary>
        public string url;
        /// <summary>
        /// 
        /// </summary>
        public Object arg;
        /// <summary>
        /// 可为 GET、POST、DELETE、PUT等,可不指定
        /// </summary>
        public string httpMethod;
        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, string> headers;
    }
}
