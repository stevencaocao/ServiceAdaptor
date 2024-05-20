using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;

namespace MSCore.Util.Swagger
{
    public static class IWebHostBuilderExtensions_Swagger
    {
        /// <summary>
        /// 使用自定义swagger，支持版本控制
        /// Program中需增加以下配置
        /// app.UseMSCoreSwaggerUI<![CDATA[<ApiVersion>]]>();
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="configuration"></param>
        /// <param name="xmlFileName"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseMSCoreSwagger<ApiVersion>(this IWebHostBuilder builder, IConfiguration configuration, string xmlFileName)
        {
            builder.ConfigureServices(delegate (IServiceCollection services)
            {
                services.AddSwaggerGen(options =>
                {

                    foreach (FieldInfo fileld in typeof(ApiVersion).GetFields())
                    {
                        if (fileld.Name == "value__")
                            continue;
                        options.SwaggerDoc(fileld.Name, new OpenApiInfo
                        {
                            Version = fileld.Name,
                            Title = configuration.GetSection("ProjectName").Value,
                            Description = $"{configuration.GetSection("ProjectDescription").Value}"
                        });

                    }
                    xmlFileName = $"{xmlFileName}.xml";
                    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName), true);
                });

            });


            return builder;
        }

        /// <summary>
        /// 添加swaggerUI的版本控制
        /// </summary>
        /// <typeparam name="ApiVersion"></typeparam>
        /// <param name="app"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseMSCoreSwaggerUI<ApiVersion>(this IApplicationBuilder app) {
            app.UseSwaggerUI(c =>
            {
                foreach (FieldInfo field in typeof(ApiVersion).GetFields())
                {
                    if (field.Name == "value__")
                        continue;
                    c.SwaggerEndpoint($"/swagger/{field.Name}/swagger.json", $"{field.Name}");
                }
            });
            return app;
        }
    }
}
