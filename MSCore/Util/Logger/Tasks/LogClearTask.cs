using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Hosting;
using MSCore.Util.Common;
using System.Linq;
using MSCore.Util.ConfigurationManager;

namespace MSCore.Util.Logger.Tasks
{
    public class LogClearTask : BackgroundService
    {
        private readonly int saveDays;
        private readonly string filePath;
        private readonly bool enable;


        public LogClearTask(IOptionsMonitor<LoggerSetting> config)
        {
            enable=config.CurrentValue.Enable;
            saveDays= config.CurrentValue.SaveDays;
            filePath= config.CurrentValue.LogFilePath;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if(!enable)
                return;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {

                    string basePath = string.IsNullOrEmpty(filePath) ? Path.Combine(AppContext.BaseDirectory, "Logs") : filePath;//.Replace("\\", "/")

                    if (Directory.Exists(basePath))
                    {
                        List<string> logPaths = IOHelper.GetDirectory(basePath).Select(v => v.FullName).ToList();

                        var deleteTime = DateTime.Now.AddDays(-1 * saveDays);

                        if (logPaths.Count != 0)
                        {
                            foreach (var logPath in logPaths)
                            {
                                //var fileInfo = new FileInfo(logPath);

                                //if (fileInfo.CreationTime < deleteTime)
                                //{
                                //    File.Delete(logPath);
                                //}
                                if (Convert.ToDateTime(logPath.Substring(logPath.Length - 10)).Date < deleteTime.Date)
                                {
                                    Directory.Delete(logPath, true);
                                }
                            }
                        }
                    }

                }
                catch
                {
                }

                await Task.Delay(1000 * 60 * 60 * 24, stoppingToken);
            }
        }
    }
}
