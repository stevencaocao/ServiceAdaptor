using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Net;

namespace ServiceAdapter.https
{
    public static class IWebHostBuilderExtensions_UseHttps
    {
        /// <summary>
        /// 使用https
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="consulConfig"></param>
        /// <returns></returns>
        public static IWebHostBuilder UseHttps(this IWebHostBuilder builder, IConfiguration configuration, string[] args)
        {
            if (Convert.ToString(configuration["https:Enable"]).ToLower().Trim() == "true")
            {
                builder.UseKestrel(options =>
                {
                    options.Listen(IPAddress.Any, args.Length != 0 ? int.Parse(args[0]) : Convert.ToInt32(configuration["https:defaultPort"]), ops =>
                    {
                        ops.UseHttps(configuration["https:Certificate:Path"], configuration["https:Certificate:Password"]);
                    });
                });
            }

            return builder;
        }
    }
}
