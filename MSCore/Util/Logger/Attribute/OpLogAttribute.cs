using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using MSCore.Util.ConfigurationManager;
using MSCore.Util.Logger.OpLog;
using MSCore.Util.Logger.OpLog.WebHelper;
using MSCore.Util.MemoryCache;
using MSCore.Util.Redis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MSCore.Util.Logger.Attribute
{
    /// <summary>
    /// 操作日志
    /// </summary>
    public class OpLogAttribute : System.Attribute, IActionFilter
    {
        /// <summary>
        /// 操作内容
        /// </summary>
        public string OperateContent { get; set; }
        /// <summary>
        /// 所属业务模块
        /// </summary>
        public string BelongModule { get; set; }
        /// <summary>
        /// 操作窗口
        /// </summary>
        public string BelongWindow { get; set; }
        /// <summary>
        /// 所属功能
        /// </summary>
        public string BelongFun { get; set; }

        private IHttpClientHelper clientHelper;

        public async void OnActionExecuted(ActionExecutedContext actionExecutedContext)
        {
            var httpContext = actionExecutedContext.HttpContext;
            var actionDescriptor = actionExecutedContext.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
            var mOpLog = new MOpLog();

            #region (x.1)获取基础信息
            mOpLog.Ip = httpContext.Connection?.RemoteIpAddress?.ToString();
            #endregion

            #region (x.2)从被调用的函数的特性上获取模块信息
            mOpLog.ModulePath = httpContext.Request.Path.Value;
            mOpLog.MethodName = actionDescriptor.ActionName;
            mOpLog.BelongModule = BelongModule;
            mOpLog.BelongWindow = BelongWindow;
            mOpLog.BelongFun = BelongFun;
            mOpLog.OperateContent = OperateContent;

            #endregion

            #region (x.3)获取登录用户信息
            string at = string.Empty, token;
            if (httpContext.Request.Headers.TryGetValue("Authorization", out var bear))
            {
                try
                {
                    token = bear.ToString();
                    if (!string.IsNullOrWhiteSpace(token) && token.Length > 7)
                        at = token.Substring(7);//过滤"Bearer "
                }
                catch
                {
                }
            }

            if (httpContext.Request.Cookies.TryGetValue("at", out token))
            {
                if (!string.IsNullOrWhiteSpace(token) && token.Length > 7)
                    at = token.Substring(7);//过滤"Bearer "
            }

            var shorToken = GenerateMD5(at);
            JObject user = null;
            if (!string.IsNullOrEmpty(shorToken))
            {
                try
                {
                    using (var redis = RedisUtil.Instance.GetRedis())
                    {
                        user = redis.GetDatabase(1).Get<JObject>(shorToken);
                    }
                }
                catch (Exception ex)
                {
                    LoggerHelper.LogInfo("redis connection exception:" + ex.Message);
                    MsCache.Instance.TryGetValue<JObject>(shorToken, out user);
                }
            }

            if (user != null)
            {
                mOpLog.UserCode = user["SystemUser"]["Id"].ToString();
                mOpLog.UserName = user["SystemUser"]["UserName"].ToString();
            }
            #endregion
            clientHelper = new HttpClientHelper();
            string remoteLogUrl = Appsettings.json.GetStringByPath("LocalLog.RemoteOpLogUrl")?.ToString();
            if (!string.IsNullOrEmpty(remoteLogUrl))
            {
                string url = httpContext.Request.IsHttps ? "https://" : "http://" + httpContext.Request.Host.ToString() + httpContext.Request.PathBase.ToString() + "/" + remoteLogUrl;
                LoggerHelper.LogInfo("url:" + url);
                await clientHelper.Post<bool>(url, JsonConvert.SerializeObject(mOpLog));//api/OpLog/Collector/Push
            }
            else
            {
                LoggerHelper.LogInfo(JsonConvert.SerializeObject(mOpLog), "OpLog");
            }

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }

        /// <summary>
        /// MD5加密方法
        /// </summary>
        /// <param name="txt">要加密的字符串</param>
        /// <returns></returns>
        private string GenerateMD5(string txt)
        {
            using (MD5 mi = MD5.Create())
            {
                byte[] buffer = Encoding.Default.GetBytes(txt);
                //开始加密
                byte[] newBuffer = mi.ComputeHash(buffer);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < newBuffer.Length; i++)
                {
                    sb.Append(newBuffer[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }

    }

}
