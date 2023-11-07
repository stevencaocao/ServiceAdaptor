using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ServiceAdapter.Logger.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapter.Logger
{
    public class LocalFileLogger : ILogger
    {
        private readonly string categoryName;
        private readonly string basePath;

        public LocalFileLogger(LoggerSetting loggerSetting, string categoryName)
        {
            this.categoryName = categoryName;
            basePath = string.IsNullOrEmpty(loggerSetting.LogFilePath) ? Path.Combine(Directory.GetCurrentDirectory(), "Logs") : loggerSetting.LogFilePath;//.Replace("\\", "/")

            if (Directory.Exists(basePath) == false)
            {
                Directory.CreateDirectory(basePath);
            }
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return default;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel != LogLevel.None)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception = null, Func<TState, Exception, string> formatter = null)
        {
            if (IsEnabled(logLevel))
            {
                if (state != null && state.ToString() != null)
                {
                    var logContent = state.ToString();

                    if (logContent != null)
                    {
                        if (exception != null)
                        {
                            var logMsg = new
                            {
                                message = logContent,
                                error = new
                                {
                                    exception?.Source,
                                    exception?.Message,
                                    exception?.StackTrace
                                }
                            };

                            logContent = JsonConvert.SerializeObject(logMsg);
                        }

                        var log = new
                        {
                            CreateTime = DateTime.UtcNow,
                            Category = categoryName,
                            Level = logLevel.ToString(),
                            Content = logContent
                        };

                        string logStr = JsonConvert.SerializeObject(log);

                        var logPath = Path.Combine(basePath, DateTime.UtcNow.ToString("yyyyMMddHH") + ".log");

                        File.AppendAllText(logPath, logStr + Environment.NewLine, Encoding.UTF8);

                    }
                }
            }
        }
    }
}
