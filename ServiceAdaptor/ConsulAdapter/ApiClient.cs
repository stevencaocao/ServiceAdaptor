using Consul;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceAdaptor;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace ServiceAdapter.ConsulAdapter
{
    public class ApiClient
    {
        private static int iIndex = 0;//暂不考虑线程安全
        public static ApiClient Instance { get; internal set; }
        public ApiClient(string ConsulAddress)
        {
            this.ConsulAddress = ConsulAddress;
        }


        /// <summary>
        ///  http://127.0.0.1:8500
        /// </summary>
        string ConsulAddress;


        #region FindServiceByServiceName

        private async static Task<CatalogService> FindServiceByServiceNameAsync(string serviceName)
        {
            using (var consulClient = new ConsulClient(a => a.Address = new Uri(Instance.ConsulAddress)))
            {
                var services = (await consulClient.Catalog.Service(serviceName)).Response;
                if (services != null && services.Any())
                {
                    //// 模拟随机一台进行请求，这里只是测试，可以选择合适的负载均衡工具或框架
                    //Random r = new Random();
                    //int index = r.Next(services.Count());
                    //var service = services.ElementAt(index);
                    //return service;

                    ////权重：每个实例能力不同，承担的压力也要不同
                    List<CatalogService> pairsList = new List<CatalogService>();
                    foreach (var pair in services.ToArray())
                    {
                        int count = int.Parse(pair.ServiceTags?[0]);//1 5 10
                        if (count <= 0)
                        {
                            count = 1;
                        }
                        for (int i = 0; i < count; i++)
                        {
                            pairsList.Add(pair);
                        }
                    }
                    //16个
                    int index=new Random(iIndex++).Next(0, pairsList.Count());
                    var service = pairsList[index];
                    Console.WriteLine("index:"+index);
                    Console.WriteLine(JsonConvert.SerializeObject(pairsList));
                    return service;
                }
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 解析服务主机名
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private static string GetServiceName(ApiRequest data)
        {
            var apiStation = data.url.Split('/')[1];
            return apiStation;
        }

        private static async Task<ApiResponse<ReturnType>> BeforeCallApi<ReturnType>(ApiRequest req)
        {
            //Uri uri = new Uri(req.url);
            //string serviceName = uri.Host;
            string serviceName = GetServiceName(req);
            var serviceInfo = await FindServiceByServiceNameAsync(serviceName);
            if (serviceInfo == null)
            {
                return new ApiResponse<ReturnType> { StatusCode = (int)HttpStatusCode.NotFound };
            }

            req.url = $"http://{serviceInfo.ServiceAddress}:{serviceInfo.ServicePort}{req.url.Replace("/" + serviceName, "")}";
            return null;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ReturnType"></typeparam>
        /// <param name="req"></param>
        /// <returns></returns>
        public static ApiResponse<ReturnType> CallApi<ReturnType>(ApiRequest req)
        {
            return CallApiAsync<ReturnType>(req).Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="ReturnType"></typeparam>
        /// <param name="req"></param>
        /// <returns></returns>
        public static async Task<ApiResponse<ReturnType>> CallApiAsync<ReturnType>(ApiRequest req)
        {
            //(x.0)BeforeCallApi
            var apiResponse = BeforeCallApi<ReturnType>(req).Result;
            if (apiResponse != null) return apiResponse;

            var httpClient = new HttpClient();
            string text = req.url;
            if (req.arg != null)
            {
                text = UrlAddParams(text, req.arg);
            }
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage(new HttpMethod(req.httpMethod ?? "GET"), text);
            if (req.arg != null)
            {
                object body = req.arg;
                byte[] array = body as byte[];
                if (array == null)
                {
                    HttpContent httpContent = body as HttpContent;
                    if (httpContent != null)
                    {
                        httpRequestMessage.Content = httpContent;
                    }
                    else
                    {
                        string content = JsonConvert.SerializeObject(req.arg);
                        httpRequestMessage.Content = new StringContent(content, Encoding.Default, "application/json");
                    }
                }
                else
                {
                    httpRequestMessage.Content = new ByteArrayContent(array);
                }
            }

            if (req.headers != null)
            {
                if (httpRequestMessage.Content != null)
                {
                    foreach (KeyValuePair<string, string> header in req.headers)
                    {
                        httpRequestMessage.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, string> header2 in req.headers)
                    {
                        httpRequestMessage.Headers.TryAddWithoutValidation(header2.Key, header2.Value);
                    }
                }
            }
            HttpResponseMessage httpResponseMessage = await httpClient.SendAsync(httpRequestMessage);
            ApiResponse<ReturnType> response = new ApiResponse<ReturnType>
            {
                StatusCode = (int)httpResponseMessage.StatusCode,
                headers = httpResponseMessage.Content?.Headers?.AsEnumerable().ToDictionary((KeyValuePair<string, IEnumerable<string>> kv) => kv.Key, (KeyValuePair<string, IEnumerable<string>> kv) => string.Join(",", kv.Value))
            };
            if (typeof(byte[]).IsAssignableFrom(typeof(ReturnType)))
            {
                response.data = (ReturnType)(object)(await httpResponseMessage.Content.ReadAsByteArrayAsync());
            }
            else
            {
                response.data = DeserializeFromString<ReturnType>(await httpResponseMessage.Content.ReadAsStringAsync());
            }

            return response;
        }

        //
        // 参数:
        //   url:
        //     不可为null,例如："http://www.abc.com"、"http://www.abc.com?k=1&m=2"
        //
        //   parameters:
        //     可为string、IDictionary、JObject
        private static string UrlAddParams(string url, object parameters)
        {
            string text = FormatUrlParams(parameters);
            if (string.IsNullOrEmpty(text))
            {
                return url;
            }

            if (0 < url?.IndexOf('?'))
            {
                return url + "&" + text;
            }

            return url + "?" + text;
        }

        //
        // 摘要:
        //     System.Web.HttpUtility.UrlEncode(param)
        //
        // 参数:
        //   param:
        private static string UrlEncode(string param)
        {
            if (string.IsNullOrEmpty(param))
            {
                return "";
            }

            try
            {
                param = HttpUtility.UrlEncode(param);
                return param;
            }
            catch
            {
                return param;
            }
        }

        //
        // 摘要:
        //     返回值demo： "a=4&b=2"
        //
        // 参数:
        //   parameters:
        //     可为string、IDictionary、JObject,例如："a=3&b=5"
        private static string FormatUrlParams(object parameters)
        {
            if (parameters == null)
            {
                return null;
            }

            if (parameters is IDictionary)
            {
                StringBuilder stringBuilder = new StringBuilder();
                foreach (DictionaryEntry item in (IDictionary)parameters)
                {
                    stringBuilder.Append(UrlEncode(item.Key.ToString())).Append("=").Append(UrlEncode(item.Value.ToString()))
                        .Append("&");
                }

                if (stringBuilder.Length > 0)
                {
                    stringBuilder.Length--;
                }

                return stringBuilder.ToString();
            }

            JObject jObject = parameters as JObject;
            if (jObject != null)
            {
                return FormatJObject(jObject);
            }

            if (parameters is string)
            {
                return (string)parameters;
            }

            return FormatJObject(JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(parameters)));
            string FormatJObject(JObject joParameters)
            {
                StringBuilder stringBuilder2 = new StringBuilder();
                foreach (KeyValuePair<string, JToken> joParameter in joParameters)
                {
                    stringBuilder2.Append(UrlEncode(joParameter.Key)).Append("=").Append(UrlEncode(joParameter.Value.ToString()))
                        .Append("&");
                }

                if (stringBuilder2.Length > 0)
                {
                    stringBuilder2.Length--;
                }

                return stringBuilder2.ToString();
            }
        }

        public const string Get = "GET";
        public const string Post = "POST";
        public const string Delete = "DELETE";
        public const string Put = "PUT";
        public const string Patch = "PATCH";
        private static Type GetUnderlyingTypeIfNullable(Type type)
        {
            return IsNullable(type) ? type.GetGenericArguments()[0] : type;
        }
        private static bool IsNullable(Type type)
        {
            return true == type?.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
        private static bool TypeIsValueTypeOrStringType(Type type)
        {
            if (type == null)
            {
                return false;
            }
            return type.IsValueType || type == typeof(string);
        }
        private static object DeserializeStruct(string value, Type type)
        {
            try
            {
                if (type == typeof(string))
                    return value;
                return Convert(value, type);
            }
            catch { }
            return DefaultValue(type);
        }
        private static object DefaultValue(Type type)
        {
            if (null == type || !type.IsValueType) return null;
            return Activator.CreateInstance(type);
        }
        private static object Convert(object value, Type type)
        {
            if (value == null)
            {
                return null;
                //throw new ArgumentNullException(nameof(value));
            }
            return System.Convert.ChangeType(value, GetUnderlyingTypeIfNullable(type));
        }

        private static T DeserializeFromString<T>(string value)
        {
            if (null == value) return default;

            Type type = typeof(T);

            if (GetUnderlyingTypeIfNullable(typeof(T)).IsEnum)
            {
                return (T)Enum.Parse(GetUnderlyingTypeIfNullable(typeof(T)), value);
            }

            if (TypeIsValueTypeOrStringType(typeof(T)))
            {
                return (T)DeserializeStruct(value, type);
            }

            return JsonConvert.DeserializeObject<T>(value);
        }
    }
}
