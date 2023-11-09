using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceB.Entities;

namespace ServiceB.Controllers
{
    /// <summary>
    /// 健康检查
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        [HttpGet("index")]
        public string Index()
        {
            return $"你正在调用ServiceB服务端口号为{Request.HttpContext.Connection.LocalPort}的Index方法";
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
            return $"你正在调用ServiceB服务端口号为{Request.HttpContext.Connection.LocalPort}的Test方法";
        }

        [HttpGet("index2")]
        public string TestService()
        {
            return $"你正在调用ServiceB服务端口号为{Request.HttpContext.Connection.LocalPort}的TestService方法";
        }

        [HttpPost("index3")]
        public UserInfo TestService([FromBody] UserInfo userInfo)
        {
            //Console.WriteLine(JsonConvert.SerializeObject(userInfo));
            userInfo.remark= $"你正在调用ServiceB服务端口号为{Request.HttpContext.Connection.LocalPort}的TestService方法";
            return userInfo;
        }
    }
}
