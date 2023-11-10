using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapter.Logger.Models
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
        public string LogFilePath { get; set; } = Path.Combine(Directory.GetCurrentDirectory(),"Logs");//.Replace("\\", "/")
    }
}
