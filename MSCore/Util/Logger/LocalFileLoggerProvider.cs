using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;

namespace MSCore.Util.Logger
{
    public class LocalFileLoggerProvider : ILoggerProvider
    {
        private readonly LoggerSetting _loggerSetting;
        private readonly ConcurrentDictionary<string, LocalFileLogger> loggers = new ConcurrentDictionary<string, LocalFileLogger>();
        public LocalFileLoggerProvider(IOptionsMonitor<LoggerSetting> optionsMonitor)
        {
            _loggerSetting = optionsMonitor.CurrentValue;
        }

        public ILogger CreateLogger(string categoryName)
        {
            return loggers.GetOrAdd(categoryName, new LocalFileLogger(_loggerSetting, categoryName));
        }

        public void Dispose()
        {
            loggers.Clear();
            GC.SuppressFinalize(this);
        }
    }
}
