using Nacos.V2;
using System;
using System.Collections.Generic;
using System.Text;

namespace ServiceAdapter.NacosAdapter
{
    public class NacosConfig: NacosSdkOptions
    {
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// 注册服务名称
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// 分组名称
        /// </summary>
        public string GroupName { get; set; } = Nacos.V2.Common.Constants.DEFAULT_GROUP;

        /// <summary>
        /// 集群名称
        /// </summary>
        public string ClusterName { get; set; } = Nacos.V2.Common.Constants.DEFAULT_CLUSTER_NAME;

        /// <summary>
        /// ip地址
        /// </summary>
        public string Ip { get; set; }

        /// <summary>
        /// 与前缀匹配的IP作为服务注册IP，如nacos.inetutils.preferred-networks的配置
        /// </summary>
        public string PreferredNetworks { get; set; }

        /// <summary>
        /// 端口
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// 权重
        /// </summary>
        public double Weight { get; set; } = 100;

        /// <summary>
        /// 元数据
        /// </summary>
        public Dictionary<string, string> Metadata { get; set; } = new Dictionary<string, string>();

        /// <summary>
        /// 是否启用接受请求
        /// </summary>
        public bool InstanceEnabled { get; set; } = true;

        /// <summary>
        /// 是否为短暂实例
        /// </summary>
        public bool Ephemeral { get; set; } = true;

        /// <summary>
        /// 是否是https
        /// </summary>
        public bool Secure { get; set; } = false;

        public Action<NacosSdkOptions> BuildSdkOptions()
        {
            return x =>
            {
                x.AccessKey = this.AccessKey;
                x.ConfigUseRpc = this.ConfigUseRpc;
                x.ContextPath = this.ContextPath;
                x.DefaultTimeOut = this.DefaultTimeOut;
                x.EndPoint = this.EndPoint;
                x.ListenInterval = this.ListenInterval;
                x.Namespace = this.Namespace;
                x.NamingLoadCacheAtStart = this.NamingLoadCacheAtStart;
                x.NamingUseRpc = this.NamingUseRpc;
                x.Password = this.Password;
                x.SecretKey = this.SecretKey;
                x.ServerAddresses = this.ServerAddresses;
                x.UserName = this.UserName;
            };
        }

    }
}
