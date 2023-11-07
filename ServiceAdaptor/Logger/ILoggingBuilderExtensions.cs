using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServiceAdapter.Logger.Models;
using ServiceAdapter.Logger.Tasks;
using System;

namespace ServiceAdapter.Logger
{
    public static class ILoggingBuilderExtensions
    {
        /// <summary>
        /// 增加日志文件
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="loggerSetting"></param>
        public static void AddLocalFileLogger(this ILoggingBuilder builder, LoggerSetting loggerSetting)
        {
            if (loggerSetting == null || !loggerSetting.Enable)
            {
                return;
            }

            builder.Services.Configure(new Action<LoggerSetting>(k =>
            {
                k.SaveDays = loggerSetting.SaveDays;
                k.Enable = loggerSetting.Enable;
                k.LogFilePath = loggerSetting.LogFilePath;
            }));
            builder.Services.AddSingleton<ILoggerProvider, LocalFileLoggerProvider>();
            builder.Services.AddSingleton<IHostedService, LogClearTask>();
        }
    }
}
