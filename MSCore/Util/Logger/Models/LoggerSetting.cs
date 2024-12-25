using System;
using System.IO;

namespace MSCore.Util.Logger
{
    public class LoggerSetting
    {
        public bool Enable { get; set; }
        /// <summary>
        /// 保存天数
        /// </summary>
        public int SaveDays { get; set; } = 7;

        /// <summary>
        /// 日志文件路径
        /// </summary>
        public string LogFilePath { get; set; } = Path.Combine(AppContext.BaseDirectory, "Logs");//.Replace("\\", "/")

        /// <summary>
        /// 日志文件根据消息等级分别存储 0表示不单独存储，1表示单独存储
        /// </summary>
        public int SingleLevelFile { get; set; }

        /// <summary>
        /// 本地sqlite 日志
        /// </summary>
        public bool SqliteLog { get; set; }

        /// <summary>
        /// 远程存储日志接口
        /// </summary>
        public string RemoteOpLogUrl { get; set; }
    }
}
