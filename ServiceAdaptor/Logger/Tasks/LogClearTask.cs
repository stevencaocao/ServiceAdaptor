using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ServiceAdapter.Common;
using ServiceAdapter.Logger.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceAdapter.Logger.Tasks
{
    public class LogClearTask : BackgroundService
    {
        private readonly int saveDays;
        private readonly string filePath;


        public LogClearTask(IOptionsMonitor<LoggerSetting> config)
        {
            saveDays = config.CurrentValue.SaveDays;
            filePath = config.CurrentValue.LogFilePath;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {

                    string basePath = string.IsNullOrEmpty(filePath) ? Path.Combine(Directory.GetCurrentDirectory(), "Logs") : filePath;//.Replace("\\", "/")

                    if (Directory.Exists(basePath))
                    {
                        List<string> logPaths = IOHelper.GetFiles(basePath).Select(v => v.FullName).ToList();

                        var deleteTime = DateTime.UtcNow.AddDays(-1 * saveDays);

                        if (logPaths.Count != 0)
                        {
                            foreach (var logPath in logPaths)
                            {
                                var fileInfo = new FileInfo(logPath);

                                if (fileInfo.CreationTimeUtc < deleteTime)
                                {
                                    File.Delete(logPath);
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
