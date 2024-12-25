using MSCore.Util.Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace MSCore.Util.Logger.OpLog.WebHelper
{
    public class HttpClientHelper : IHttpClientHelper
    {
        /// <summary>
        /// 通用post方法
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="data">请求参数</param>
        /// <returns></returns>
        public Task<ApiResponse<T>> Post<T>(string url, string data, Dictionary<string, string> header = null)
        {
            var result = new ApiResponse<T>
            {
                StatusCode = 500,
                data = default
            };
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.Timeout = new TimeSpan(0, 0, 20);//设置超时时长20秒
                    HttpContent content = new StringContent(data);
                    content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Connection.Add("keep-alive");
                    if (header != null)
                    {
                        foreach (var v in header)
                        {
                            client.DefaultRequestHeaders.Add(v.Key, v.Value);
                        }
                    }
                    Task<HttpResponseMessage> res = client.PostAsync(url, content);
                    var obj = res.Result.Content.ReadAsStringAsync().Result;
                    var success = ((JObject)obj)["success"];
                    Dictionary<string, string> returnHeader = new Dictionary<string, string>();
                    foreach (var x in res.Result.Content.Headers)
                    {
                        returnHeader.Add(x.Key, string.Join(";", x.Value.ToArray()));
                    }

                    result = new ApiResponse<T>
                    {
                        StatusCode = (int)res.Result.StatusCode,
                        data = JsonConvert.DeserializeObject<T>((success != null && success.ToString() == "true") ? ((JObject)obj)["obj"].ToString() : obj),
                        headers = returnHeader
                    };
                }
                catch (Exception ex)
                {
                    result.message = ex.Message;
                    LoggerHelper.LogError(ex.ToString());
                }
                finally
                {
                    client.Dispose();
                }
            }
            return Task.FromResult(result);
        }

        /// <summary>
        /// 通用post方法
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="args">请求参数</param>
        /// <returns></returns>
        public Task<ApiResponse<T>> Get<T>(string url, object args, Dictionary<string, string> header = null)
        {
            var result = new ApiResponse<T>
            {
                StatusCode = 500,
                data = default
            };
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.Timeout = new TimeSpan(0, 0, 20);//设置超时时长20秒
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                    if (header != null)
                    {
                        foreach (var v in header)
                        {
                            client.DefaultRequestHeaders.Add(v.Key, v.Value);
                        }
                    }
                    Task<HttpResponseMessage> res = client.GetAsync(UrlAddParams(url, args));
                    var obj = res.Result.Content.ReadAsStringAsync().Result;
                    var success = ((JObject)obj)["success"];
                    Dictionary<string, string> returnHeader = new Dictionary<string, string>();
                    foreach (var x in res.Result.Content.Headers)
                    {
                        returnHeader.Add(x.Key, string.Join(";", x.Value.ToArray()));
                    }
                    result = new ApiResponse<T>
                    {
                        StatusCode = (int)res.Result.StatusCode,
                        data = JsonConvert.DeserializeObject<T>((success != null && success.ToString() == "true") ? ((JObject)obj)["obj"].ToString() : obj),
                        headers = returnHeader
                    };
                }
                catch (Exception ex)
                {
                    result.message = ex.Message;
                    LoggerHelper.LogError(ex.ToString());
                }
                finally
                {
                    client.Dispose();
                }
            }
            return Task.FromResult(result);
        }

        private string UrlAddParams(string url, object parameters)
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
        private string FormatUrlParams(object parameters)
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

            return FormatJObject(parameters.ConvertBySerialize<JObject>());

        }
        private static string FormatJObject(JObject joParameters)
        {
            StringBuilder stringBuilder2 = new StringBuilder();
            foreach (KeyValuePair<string, JToken> joParameter in joParameters)
            {
                stringBuilder2.Append(UrlEncode(joParameter.Key)).Append("=").Append(UrlEncode(joParameter.Value.ConvertToString()))
                    .Append("&");
            }

            if (stringBuilder2.Length > 0)
            {
                stringBuilder2.Length--;
            }

            return stringBuilder2.ToString();
        }
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

    }
}
