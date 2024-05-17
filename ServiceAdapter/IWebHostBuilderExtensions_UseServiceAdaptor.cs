using Consul;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nacos.V2.DependencyInjection;
using Newtonsoft.Json;
using ServiceAdapter.NacosAdapter;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ServiceAdapter
{
    public class AutoRequestServicesStartupFilter : IStartupFilter
    {
        public AutoRequestServicesStartupFilter(Action<IApplicationBuilder> beforeConfig = null, Action<IApplicationBuilder> afterConfig = null)
        {
            this.beforeConfig = beforeConfig;
            this.afterConfig = afterConfig;
        }
        public Action<IApplicationBuilder> beforeConfig;

        public Action<IApplicationBuilder> afterConfig;

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return delegate (IApplicationBuilder builder)
            {
                beforeConfig?.Invoke(builder);
                next(builder);
                afterConfig?.Invoke(builder);
            };
        }
    }
    internal sealed class WebHostBuilderExtensions { }
    public static class IWebHostBuilderExtensions_UseServiceAdaptor
    {
        /// <summary>
        /// 将服务注册到注册中心
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="consulConfig"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseServiceAdaptor(this IWebHostBuilder builder, IConfiguration configuration)
        {
            int serviceCenter = 1;//当前使用的服务中心1：consul 2:nacos
            ServiceCenter config=configuration.GetSection("servicecenter").Get<ServiceCenter>();

            builder.ConfigureServices(delegate (IServiceCollection services)
            {
                var logger = services.BuildServiceProvider().GetRequiredService<ILogger<WebHostBuilderExtensions>>();

                if (config.consul == null || !config.consul.Enable)
                {
                    logger.LogInformation("------------------未配置或未启用consul，尝试使用nacos...");
                    if (config.nacos == null || !config.nacos.Enable)
                    {
                        logger.LogInformation("------------------未配置或未启用nacos，将启动单服务运行...");
                        return;
                    }
                    serviceCenter = 2;
                }
                switch (serviceCenter)
                {
                    case 1:
                        #region 注册到consul

                        services?.AddTransient((Func<IServiceProvider, IStartupFilter>)((IServiceProvider m) => new AutoRequestServicesStartupFilter(app =>
                        {
                            var serviceProvider = app.ApplicationServices;
                            IApplicationLifetime appLife = serviceProvider.GetRequiredService<IApplicationLifetime>();

                            var consulConfig = config.consul;
                            logger.LogInformation("------------------[ServiceAdaptor.Consul]开始注册到consul... ");

                            logger.LogInformation("------------------[ServiceAdaptor.Consul]配置：\n" + JsonConvert.SerializeObject(consulConfig));

                            ApiClient.Instance = new ConsulAdapter.ApiClient(consulConfig.ConsulEndpoint);

                            var consulClient = new ConsulClient(c => c.Address = new Uri(consulConfig.ConsulEndpoint));
                            var registration = new AgentServiceRegistration()
                            {
                                ID = consulConfig.ServiceId, //"service-" + Guid.NewGuid(),//唯一ID
                                Name = consulConfig.GroupName,//组名称
                                Address = consulConfig.ServiceHost,//提供服务IP地址
                                Port = consulConfig.ServicePort,//实例端口
                                Tags = consulConfig.Tags.Split(','),//标签
                                Check = new AgentServiceCheck()
                                {
                                    Interval = TimeSpan.FromSeconds(10),//健康检查间隔10s一次
                                    HTTP = $"{Uri.UriSchemeHttp}://{consulConfig.ServiceHost}:{consulConfig.ServicePort}{consulConfig.HealthCheckUrl}",
                                    Timeout = TimeSpan.FromSeconds(5),//监测等待时间                            
                                    DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(1)//服务启动多久后注册
                                }
                            };
                            try
                            {
                                var result = consulClient.Agent.ServiceRegister(registration).GetAwaiter().GetResult();

                                if ((result?.StatusCode) != System.Net.HttpStatusCode.OK)
                                {
                                    logger.LogInformation("------------------[ServiceAdaptor.Consul]注册失败!");
                                    Task.Run(() =>
                                    {
                                        appLife.StopApplication();
                                    });
                                    return;
                                }
                            }
                            catch (Exception ex)
                            {
                                logger.LogError(ex.ToString());
                                logger.LogInformation("------------------[ServiceAdaptor.Consul]注册失败!");
                                Task.Run(() =>
                                {
                                    appLife.StopApplication();
                                });
                                return;
                            }

                            #region 解绑注册
                            appLife.ApplicationStopped.Register(() =>
                            {
                                consulClient.Agent.ServiceDeregister(consulConfig.ServiceId).Wait();
                                logger.LogInformation("------------------[ServiceAdaptor.Consul]已注销服务");
                            });
                            #endregion

                            #region 添加健康检查接口
                            app.Map(consulConfig.HealthCheckUrl, s =>
                            {
                                s.Run(async context =>
                                {
                                    await context.Response.WriteAsync("ok");
                                });
                            });

                            #endregion

                            logger.LogInformation("------------------[ServiceAdaptor.Consul]注册成功!");

                        })));

                        #endregion
                        break;
                    case 2:
                        #region 注册到nacos
                       
                        services.Configure<NacosConfig>(configuration.GetSection("servicecenter:nacos"));
                        services.AddNacosV2Naming(configuration,null,"servicecenter:nacos");

                        services.AddHostedService<RegSvcBgTask>();

                        #endregion
                        break;
                    default: break;
                }


            });


            return builder;
        }
    }
}