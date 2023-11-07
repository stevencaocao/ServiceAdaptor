using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ServiceB.Controllers
{
    /// <summary>
    /// 健康检查
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class HealthController : ControllerBase
    {
        [HttpGet("index")]
        public string Index()
        {
            return $"你正在调用ServiceB服务端口号为{Request.HttpContext.Connection.LocalPort}的Index方法";
        }

        [HttpGet("test")]
        public string Test()
        {
            return $"你正在调用ServiceB服务端口号为{Request.HttpContext.Connection.LocalPort}的Test方法";
        }
    }
}
