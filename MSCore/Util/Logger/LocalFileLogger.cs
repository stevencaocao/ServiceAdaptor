using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;

namespace MSCore.Util.Logger
{
    public class LocalFileLogger : ILogger
    {
        private readonly string categoryName;
        private readonly string basePath;
        private readonly LoggerSetting _loggerSetting;

        public LocalFileLogger(LoggerSetting loggerSetting, string categoryName)
        {
            _loggerSetting = loggerSetting;
            this.categoryName = categoryName;
            basePath = string.IsNullOrEmpty(loggerSetting.LogFilePath) ? Path.Combine(AppContext.BaseDirectory, "Logs") : loggerSetting.LogFilePath;
            basePath = Path.Combine(basePath, DateTime.Now.ToString("yyyy-MM-dd"));
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
                            CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
                            Type = "AutoInjection",
                            Category = categoryName,
                            Level = logLevel.ToString(),
                            Content = logContent
                        };

                        string logStr = JsonConvert.SerializeObject(log);

                        var logPath = Path.Combine(basePath, DateTime.Now.ToString("yyyyMMddHH") + ".log");
                        if (_loggerSetting.SingleLevelFile == 1)
                        {
                            logPath = Path.Combine(basePath, DateTime.Now.ToString("yyyyMMddHH") + "-" + logLevel + ".log");
                        }

                        if (Directory.Exists(basePath) == false)
                        {
                            Directory.CreateDirectory(basePath);
                        }
                        lock (this)
                        {
                            File.AppendAllText(logPath, logStr + Environment.NewLine, Encoding.UTF8);
                        }
                    }
                }
            }
        }
    }
}
