using System;
using System.Collections.Generic;
using global::Nacos.V2.Naming.Core;
using global::Nacos.V2;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceAdapter.NacosAdapter
{
    public class RegSvcBgTask : IHostedService, IDisposable
    {
        private static readonly string MetadataNetVersion = "DOTNET_VERSION";
        private static readonly string MetadataHostOs = "HOST_OS";
        private static readonly string MetadataSecure = "secure";

        private readonly ILogger _logger;
        private readonly INacosNamingService _svc;
        private readonly IFeatureCollection _features;
        private NacosConfig _options;

        private IEnumerable<Uri> uris = null;

        public RegSvcBgTask(ILogger<RegSvcBgTask> logger,
             INacosNamingService svc, IServer server,
        IOptionsMonitor<NacosConfig> optionsAccs)
        {
            _logger = logger;
            _svc = svc;
            _options = optionsAccs.CurrentValue;
            _features = server.Features;
        }

        public void Dispose()
        {
            Console.WriteLine("Dispose");
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_options.Enable)
            {
                _logger.LogInformation("------------------未配置或未启用nacos，将启动单服务运行...");
                return;
            }
            ServiceAdapter.ApiClient.Instance = new ApiClient(_svc);

            uris = UriTool.GetUri(_features, _options.Ip, _options.Port, _options.PreferredNetworks);

            var metadata = new Dictionary<string, string>()
            {
                { PreservedMetadataKeys.REGISTER_SOURCE, $"ASPNET_CORE" },
                { MetadataNetVersion, Environment.Version.ToString() },
                { MetadataHostOs, Environment.OSVersion.ToString() },
            };

            if (_options.Secure) metadata[MetadataSecure] = "true";

            foreach (var item in _options.Metadata)
            {
                if (!metadata.ContainsKey(item.Key))
                {
                    metadata.Add(item.Key, item.Value);
                }
            }

            foreach (var uri in uris)
            {
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        var instance = new Nacos.V2.Naming.Dtos.Instance
                        {
                            Ephemeral = _options.Ephemeral,
                            ServiceName = _options.ServiceName,
                            ClusterName = _options.ClusterName,
                            Enabled = _options.InstanceEnabled,
                            Healthy = true,
                            Ip = uri.Host,
                            Port = uri.Port,
                            Weight = _options.Weight,
                            Metadata = metadata,
                            InstanceId = ""
                        };

                        _logger.LogInformation("------------------注册到nacos, 【{0}】", instance);

                        await _svc.RegisterInstance(_options.ServiceName, _options.GroupName, instance).ConfigureAwait(false);
                        _logger.LogInformation("------------------注册成功！");
                        break;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "------------------注册到nacos失败, count = {0}", i + 1);
                    }
                }
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (_options.Enable)
            {
                _logger.LogWarning("------------------从nacos注销服务, serviceName={0}", _options.ServiceName);

                foreach (var uri in uris)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            _logger.LogWarning("------------------开始注销");
                            await _svc.DeregisterInstance(_options.ServiceName, _options.GroupName, uri.Host, uri.Port, _options.ClusterName).ConfigureAwait(false);
                            _logger.LogWarning("------------------已注销");
                            break;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "------------------从nacos注销服务失败, count = {0}", i + 1);
                        }
                    }
                }
            }
        }
    }
}
