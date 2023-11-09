using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceA.ServiceProvider;
using ServiceA.ServiceProvider.Contract;

namespace ServiceA.Controllers
{
    /// <summary>
    /// 健康检查
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        ILogger<TestController> logger;
        public TestController(ILogger<TestController> _logger)
        {
            logger = _logger;
        }
        /// <summary>
        /// 正常接口调用测试
        /// </summary>
        /// <returns></returns>
        [HttpGet("index")]
        public string Index()
        {
            return $"你正在调用ServiceA服务端口号为{Request.HttpContext.Connection.LocalPort}的Index方法，{DateTime.Now}";
        }
        /// <summary>
        /// 耗时的接口，测试熔断
        /// </summary>
        /// <returns></returns>
        [HttpGet("index1")]
        public string Test()
        {
            Console.WriteLine(Math.Round(10.0) + ":1");
            Thread.Sleep(5000);
            Console.WriteLine(Math.Round(10.0) + ":2");
            return $"你正在调用ServiceA服务端口号为{Request.HttpContext.Connection.LocalPort}的Test方法";
        }
        /// <summary>
        /// 测试A服务调用B服务
        /// </summary>
        /// <returns></returns>
        [HttpGet("index2")]
        public object CallOtherService()
        {
            UserInfo userInfo = ServiceProviderService.PostTestServiceByServiceB(new ServiceProvider.Contract.UserInfo() { name = "张三", age = 18 }).Result;
            var t = new
            {
                date = DateTime.Now,
                msg = $"你正在调用ServiceA服务端口号为{Request.HttpContext.Connection.LocalPort}的CallOtherService方法",
                getService = ServiceProviderService.GetTestServiceByServiceB().Result,
                postServiceWithBody =JsonConvert.SerializeObject(ServiceProviderService.PostTestServiceByServiceB(new ServiceProvider.Contract.UserInfo() { name = "张三", age = 18, roles = new List<string>(),remark="" }).Result)
            };

            return t;
        }
    }
}
