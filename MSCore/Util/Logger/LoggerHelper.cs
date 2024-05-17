using MSCore.Util.ConfigurationManager;
using System;
using System.IO;
using System.Threading;

namespace MSCore.Util.Logger
{
    public static class LoggerHelper
    {
        private static string _logFileName;

        private static EnumLogLevel _logLevel;

        /// <summary>
        /// 是否输出日志到文件
        /// </summary>
        private static bool _logEnable;

        //
        // 摘要:
        //     日志文件根据消息等级分别存储 0表示不单独存储，1表示单独存储
        private static string singleLevelFile;

        //
        // 摘要:
        //     记录日志文件日期，每天的日志单独放一个文件夹
        public static string LogFileDate;

        //
        // 摘要:
        //     日志对象
        //private static Logger _logger;

        //
        // 摘要:
        //     日志服务
        //
        // 参数:
        //   logEanble
        //     是否启用日志输出
        //   logFileName:
        //     日志文件路径
        //
        //   logLevel:
        //     日志等级
        static LoggerHelper()
        {
            string logFilePath = Appsettings.json.GetStringByPath("LocalLog.LogFilePath")?.ToString();
            string level = Appsettings.json.GetStringByPath("LocalLog.LogLevel")?.ToString();
            string enable = Appsettings.json.GetStringByPath("LocalLog.Enable");
            bool logEnable = string.IsNullOrEmpty(enable) ? false : Convert.ToBoolean(enable);
            EnumLogLevel logLevel = string.IsNullOrEmpty(level) ? EnumLogLevel.Warning : (EnumLogLevel)Enum.Parse(typeof(EnumLogLevel), level);
            logFilePath = Appsettings.AbsPath(string.IsNullOrEmpty(logFilePath) ? Path.Combine("Log", DateTime.Now.ToString("yyyy-MM-dd") + ".txt") : Path.Combine(logFilePath, DateTime.Now.ToString("yyyy-MM-dd") + ".txt"));
            singleLevelFile = Appsettings.json.GetStringByPath("LocalLog.SingleLevelFile")?.ToString() ?? "0";

            _logFileName = logFilePath;
            _logLevel = logLevel;
            _logEnable = logEnable;

            if (!_logEnable)
            {
                return;
            }
            LogFileDate = Path.GetFileNameWithoutExtension(_logFileName);
            if (!File.Exists(_logFileName))
            {
                string directoryName = Path.GetDirectoryName(_logFileName);
                if (!Directory.Exists(directoryName))
                {
                    Directory.CreateDirectory(directoryName);
                }
            }
        }

        //
        // 摘要:
        //     记录调试日志
        //
        // 参数:
        //   message:
        //     消息内容
        //
        //   mark:
        //     标记，日志将记录在同一个标记文件下
        public static void LogDebug(string message, string mark = "")
        {
            Log(EnumLogLevel.Debug, message, mark);
        }

        //
        // 摘要:
        //     记录消息日志
        //
        // 参数:
        //   message:
        //     消息内容
        //
        //   mark:
        //     标记，日志将记录在同一个标记文件下
        public static void LogInfo(string message, string mark = "")
        {
            Log(EnumLogLevel.Info, message, mark);
        }

        //
        // 摘要:
        //     记录警告日志
        //
        // 参数:
        //   message:
        //     消息内容
        //
        //   mark:
        //     标记，日志将记录在同一个标记文件下
        public static void LogWarning(string message, string mark = "")
        {
            Log(EnumLogLevel.Warning, message, mark);
        }

        //
        // 摘要:
        //     记录错误日志
        //
        // 参数:
        //   message:
        //     消息内容
        //
        //   mark:
        //     标记，日志将记录在同一个标记文件下
        public static void LogError(string message, string mark = "")
        {
            Log(EnumLogLevel.Error, message, mark);
        }

        //
        // 摘要:
        //     记录致命消息
        //
        // 参数:
        //   message:
        //     消息内容
        //
        //   mark:
        //     标记，日志将记录在同一个标记文件下
        public static void LogFatal(string message, string mark = "")
        {
            Log(EnumLogLevel.Fatal, message, mark);
        }

        //
        // 摘要:
        //     记录日志
        //
        // 参数:
        //   logLevel:
        //
        //   message:
        //     消息内容
        //
        //   mark:
        //     标记，日志将记录在同一个标记文件下
        private static void Log(EnumLogLevel logLevel, string message, string mark)
        {
            string value = string.Format("{0} [{1}] [Thread {2}] {3}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), logLevel, Thread.CurrentThread.ManagedThreadId, message);
            Console.WriteLine(value);
            if (logLevel < _logLevel)
            {
                return;
            }
            if (!_logEnable)
            {
                return;
            }

            string text = _logFileName;
            if (LogFileDate != DateTime.Now.ToString("yyyy-MM-dd"))
            {
                text = _logFileName.Replace(LogFileDate, DateTime.Now.ToString("yyyy-MM-dd") ?? "");
                LogFileDate = DateTime.Now.ToString("yyyy-MM-dd");
                _logFileName = text;
            }

            if (!string.IsNullOrEmpty(mark))
            {
                text = _logFileName.Replace(LogFileDate, LogFileDate + "-" + mark);
            }
            else if (singleLevelFile == "1")
            {
                text = _logFileName.Replace(LogFileDate, $"{LogFileDate}-{logLevel}");
            }

            using (StreamWriter streamWriter = new StreamWriter(text, append: true))
            {
                streamWriter.WriteLine(value);
            }
        }
    }

}
