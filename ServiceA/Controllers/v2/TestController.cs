using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MSCore.EntityFramework;
using MSCore.Util.Logger;
using ServiceA.BASE;
using ServiceA.Entity;

namespace ServiceA.Controllers.v2
{
    [Route("api/v2/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = nameof(ApiVersion.V2))]
    public class TestController : ControllerBase
    {
        ILogger<TestController> logger;
        private DBContext _dbContext;

        private IRepository<HospitalInfo> _repositoryHos;
        private IUnitOfWork _unitOfWork;

        public TestController(ILogger<TestController> _logger, DBContext dBContext, IUnitOfWork unitOfWork)
        {
            logger = _logger;
            _dbContext = dBContext;
            _repositoryHos = new Repository<HospitalInfo>(dBContext);
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// 正常接口调用测试
        /// </summary>
        /// <returns></returns>
        [HttpGet("index")]
        public string Index()
        {
            string msg = $"你正在调用ServiceA服务端口号为{Request.HttpContext.Connection.LocalPort}的Index方法，{DateTime.Now}";
            LoggerHelper.LogInfo(msg);
            logger.LogInformation("new:" + msg);
            logger.LogError(msg);
            return msg;
        }
    }
}
