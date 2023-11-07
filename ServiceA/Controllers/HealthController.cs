using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceAdapter.ConsulAdapter;

namespace ServiceA.Controllers
{
    /// <summary>
    /// 健康检查
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        ILogger<HealthController> logger;
        public HealthController(ILogger<HealthController> _logger)
        {
            logger = _logger;
        }
        [HttpGet("index")]
        public string Index()
        {
            return $"你正在调用ServiceA服务端口号为{Request.HttpContext.Connection.LocalPort}的Index方法，{DateTime.Now}";
        }
        [HttpGet("test")]
        public string Test()
        {
            Console.WriteLine(Math.Round(10.0) + ":1");
            Thread.Sleep(3000);
            Console.WriteLine(Math.Round(10.0) + ":2");
            return $"你正在调用ServiceA服务端口号为{Request.HttpContext.Connection.LocalPort}的Test方法";
        }
        /// <summary>
        /// 测试DevOps服务调用Basic服务
        /// </summary>
        /// <returns></returns>
        [HttpGet("callbasic")]
        public object CallBasic()
        {
            var t = new
            {
                date = DateTime.Now,
                msg = $"你正在调用ServiceA服务端口号为{Request.HttpContext.Connection.LocalPort}的CallBasic方法",
                Basic = ApiClient.CallApiAsync<string>(new ServiceAdaptor.ApiRequest() { url = "/ServiceB/api/Health/test", httpMethod = ApiClient.Get }).Result.data
            };

            return t;
        }
    }
}
