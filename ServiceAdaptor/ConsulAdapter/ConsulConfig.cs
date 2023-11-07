using System;

namespace ServiceAdapter.ConsulAdapter
{
    public class ConsulConfig
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// consul的地址。如 http://127.0.0.1:8500
        /// </summary>
        public string ConsulEndpoint { get; set; }

        /// <summary>
        /// 提供的服务的地址，如 127.0.0.1
        /// </summary>
        public string ServiceHost { get; set; }
        /// <summary>
        /// 提供的服务的端口号
        /// </summary>
        public int ServicePort { get; set; }

        private string serviceName;

        /// <summary>
        /// 提供的服务的名称，如 ServiceProvider
        /// </summary>
        public string ServiceName { get { return serviceName; } set { serviceName = value; serviceId= $"Service_{ServiceName}_{Guid.NewGuid()}"; } }

        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 标签
        /// </summary>
        public string Tags { get; set; }

        private string serviceId;
        /// <summary>
        /// 服务id
        /// </summary>
        public string ServiceId { get { return serviceId; } set { serviceId = value; } }
        /// <summary>
        /// 健康检查地址
        /// </summary>
        public string HealthCheckUrl { get; set; } = "/api/health/healthcheck";
    }
}
