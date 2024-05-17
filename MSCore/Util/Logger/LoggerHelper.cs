using Dm;
using MSCore.Util.ConfigurationManager;
using Newtonsoft.Json;
using System;
using System.IO;

namespace MSCore.Util.Logger
{
    public static class LoggerHelper
    {
        /// <summary>
        /// 日志文件名字，全路径
        /// </summary>
        private static string _logFileName;
        /// <summary>
        /// 日志文件父路径，全路径
        /// </summary>
        private static string _logBasePath;

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
        //     记录日志文件日期，每小时的日志单独放一个文件
        public static string LogFileDate;

        static LoggerHelper()
        {
            string logFilePath = Appsettings.json.GetStringByPath("LocalLog.LogFilePath")?.ToString();
            string level = Appsettings.json.GetStringByPath("LocalLog.LogLevel")?.ToString();
            string enable = Appsettings.json.GetStringByPath("LocalLog.Enable");
            bool logEnable = string.IsNullOrEmpty(enable) ? false : Convert.ToBoolean(enable);
            EnumLogLevel logLevel = string.IsNullOrEmpty(level) ? EnumLogLevel.None : (EnumLogLevel)Enum.Parse(typeof(EnumLogLevel), level);
            _logBasePath = string.IsNullOrEmpty(logFilePath) ? Appsettings.AbsPath("Logs") : Appsettings.AbsPath(logFilePath);
            singleLevelFile = Appsettings.json.GetStringByPath("LocalLog.SingleLevelFile")?.ToString() ?? "0";

            InitFileDirectory();

            _logLevel = logLevel;
            _logEnable = logEnable;

            if (!_logEnable)
            {
                return;
            }
        }

        /// <summary>
        /// 初始化日志文件夹
        /// </summary>
        private static void InitFileDirectory()
        {
            DateTime dateTime = DateTime.Now;
            _logFileName = Path.Combine(_logBasePath, dateTime.ToString("yyyy-MM-dd"), dateTime.ToString("yyyyMMddHH") + ".log");
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
            Log(EnumLogLevel.Information, message, mark);
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
        public static void LogCritical(string message, string mark = "")
        {
            Log(EnumLogLevel.Critical, message, mark);
        }

        //
        // 摘要:
        //     记录详细消息，包含敏感信息
        //
        // 参数:
        //   message:
        //     消息内容
        //
        //   mark:
        //     标记，日志将记录在同一个标记文件下
        public static void LogTrace(string message, string mark = "")
        {
            Log(EnumLogLevel.Trace, message, mark);
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
            var log = new
            {
                CreateTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss:ffff"),
                Type = "Manal",
                Category = "",
                Level = logLevel.ToString(),
                Content = message
            };

            string value = JsonConvert.SerializeObject(log);

            Console.WriteLine(value);
            if (logLevel < _logLevel)
            {
                return;
            }
            if (!_logEnable)
            {
                return;
            }
            InitFileDirectory();
            string text = _logFileName;
            //if (LogFileDate != DateTime.Now.ToString("yyyyMMddHH"))
            //{
            //    text = _logFileName.Replace(LogFileDate, DateTime.Now.ToString("yyyyMMddHH") ?? "");
            //    LogFileDate = DateTime.Now.ToString("yyyyMMddHH");
            //    _logFileName = text;
            //}

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
            _logFileName = string.Empty;
        }
    }

}
