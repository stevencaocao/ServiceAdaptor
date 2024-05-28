using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MSCore.Util.Logger;
using Newtonsoft.Json;
using ServiceA.Entity;
using MSCore.EntityFramework;
using ServiceA.ServiceProvider;
using ServiceA.ServiceProvider.Contract;
using MSCore.Util.ConfigurationManager;
using ServiceA.BASE;
using MSCore.Util.Redis;

namespace ServiceA.Controllers.v1
{
    /// <summary>
    /// 健康检查
    /// </summary>
    [Route("api/v1/[controller]")]
    [ApiController]
    [ApiExplorerSettings(GroupName = nameof(ApiVersion.V1))]
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

        [HttpGet("redistest")]
        public object RedisTest()
        {
            object result = null;
            using (var redis = RedisUtil.Instance.GetRedis())
            {
                redis.GetDatabase(1).Set(new { name = "张三", age = 18 }, TimeSpan.FromSeconds(160), "hkey");
                result = redis.GetDatabase(1).Get<person>("hkey");
            }
            return result;
        }

        public class person
        {
            public string name { get; set; }
            public string age { get; set; }
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
                postServiceWithBody = JsonConvert.SerializeObject(ServiceProviderService.PostTestServiceByServiceB(new ServiceProvider.Contract.UserInfo() { name = "张三", age = 18, roles = new List<string>(), remark = "" }).Result)
            };

            return t;
        }

        /// <summary>
        /// hello
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "user")]
        [HttpGet("hello")]
        public Object hello()
        {
            // 返回成功信息，写出token
            return new { success = true, data = "adfad", message = "hello" };
        }

        [HttpGet("getHospitals")]
        public List<HospitalInfo> GetHospitals()
        {

            return _dbContext.hospitals.ToList();
        }

        [HttpGet("getHospitalsBySql")]
        public List<HospitalInfo> GetHospitalsBySql()
        {
            return _dbContext.ExecSQL<HospitalInfo>(string.Format("select * from {0}hospital", Appsettings.DatabasePrefix));
        }

        [HttpPost("AddHospital")]
        public List<HospitalInfo> AddHospital()
        {
            List<HospitalInfo> result = null;
            _unitOfWork.BeginTransaction();
            try
            {
                HospitalInfo t = new HospitalInfo();
                t.Name = "test5";
                _dbContext.hospitals.Add(t);

                var hs = _dbContext.hospitals.Where(v => v.Code == "AA").FirstOrDefault();
                hs.TelNo = "30000000";
                _dbContext.hospitals.Update(hs);

                List<HospitalInfo> lsthospitals = new List<HospitalInfo>(){
            new HospitalInfo { Name = "test6" },
            new HospitalInfo { Name = "test7" }
            };
                _dbContext.hospitals.AddRange(lsthospitals);

                t = new HospitalInfo();
                t.Name = "test8";
                _repositoryHos.Insert(t);

                _repositoryHos.Update(new HospitalInfo { Id = 47, TelNo = "9999" });

                _repositoryHos.Delete(_dbContext.hospitals.First(v => v.Code == "bb"));

                _repositoryHos.ExcuteSql("update hospital set name='test1111' where name='test2'");

                var hos = _repositoryHos.Get(v => v.Code == "AA").FirstOrDefault();
                hos.Address = "400000";
                _repositoryHos.Update(hos);


                _unitOfWork.CommitTransaction();
                result = _dbContext.hospitals.ToList();
            }
            catch (Exception ex)
            {
                _unitOfWork.RollbackTransaction();
            }
            finally { _unitOfWork.Dispose(); }

            return result;
        }


    }
}
