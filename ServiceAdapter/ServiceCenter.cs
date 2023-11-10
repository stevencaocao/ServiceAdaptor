using ServiceAdapter.ConsulAdapter;
using ServiceAdapter.NacosAdapter;

namespace ServiceAdapter
{
    public class ServiceCenter
    {
        /// <summary>
        /// consul配置
        /// </summary>
        public ConsulConfig consul { get; set; }

        /// <summary>
        /// Nacos配置
        /// </summary>
        public NacosConfig nacos { get; set; }
    }
}
