using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nacos.V2.Common;
using Nacos.V2.Naming.Cache;
using Nacos.V2.Naming.Core;
using Nacos.V2.Naming.Dtos;
using Nacos.V2.Naming.Event;
using Nacos.V2.Naming.Remote;
using Nacos.V2.Naming.Utils;
using Nacos.V2.Remote;
using Nacos.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core.Logging;
using Nacos.V2.Naming.Backups;
using Nacos.V2.Utils;
using System.Collections.Concurrent;
using System.IO;

namespace ServiceAdapter.NacosAdapter
{
    public class Listeners
    {
        /// <summary>
        /// 分组名称
        /// </summary>
        public string Group { get; set; }

        /// <summary>
        /// 数据ID
        /// </summary>
        public string DataId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool Optional { get; set; }
    }
}
