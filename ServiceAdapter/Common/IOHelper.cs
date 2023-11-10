using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAdapter.Common
{
    /// <summary>
    /// 常用IO操作类
    /// </summary>
    public class IOHelper
    {
        /// <summary>
        /// 系统路径转换为web路径地址
        /// </summary>
        /// <param name="fullPath">文件完整地址</param>
        /// <param name="AppPath">应用地址</param>
        /// <param name="dHear">是否去掉首部的分隔符</param>
        /// <returns></returns>
        public static string WebPathParse(string fullPath, string appPath, bool dHear)
        {
            string sys = Path.DirectorySeparatorChar.ToString();
            string web = @"/";//web的分隔符
            if (fullPath.StartsWith(appPath))
            {
                string webPath = fullPath.Remove(0, appPath.Length);
                webPath = webPath.Replace(sys, web);
                if (webPath.StartsWith(web) == false)
                {
                    webPath = web + webPath;
                }
                if (dHear)
                {
                    webPath = IOHelper.RemoveHead(webPath, web);
                }
                return webPath;
            }
            else
            {
                return "";
            }
        }
        /// <summary>
        /// web路径地址转换为系统路径
        /// </summary>
        /// <param name="appPath">应用路径</param>
        /// <param name="webPath">web路径</param>
        /// <param name="dHear">是否去掉首部的路径分隔符</param>
        /// <returns></returns>
        public static string SysPathParse(string appPath, string webPath, bool dHear)
        {
            string sys = Path.DirectorySeparatorChar.ToString();
            string web = @"/";//web的分隔符
            webPath = webPath.Replace(web, sys);
            if (dHear)
            {
                webPath = IOHelper.RemoveHead(webPath, sys);
            }
            string result = "";
            if (appPath.EndsWith(sys))
            {
                if (webPath.StartsWith(sys))
                {
                    result = appPath + webPath.Remove(0, 1);
                }
                else
                {
                    result = appPath + webPath;
                }
            }
            else
            {
                if (webPath.StartsWith(sys))
                {
                    result = appPath + webPath;
                }
                else
                {
                    result = appPath + sys + webPath;
                }
            }
            return result;
        }
        /// <summary>
        /// 路径解析转换,转化成符合当前系统的路径符号
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="flag">（路径的类型）1：windows \ 2：linux /（linux和web网页的分隔相符）</param>
        /// <param name="dHear">是否去掉首部的路径分隔符</param>
        /// <returns></returns>
        public static string PathParse(string path, int flag, bool dHear)
        {
            string win = @"\";
            string linux = @"/";
            string sys = Path.DirectorySeparatorChar.ToString();
            if (flag == 1)
            {
                path = path.Replace(win, sys);
            }
            else if (flag == 2)
            {
                path = path.Replace(linux, sys);
            }
            if (dHear)
            {
                path = IOHelper.RemoveHead(path, sys);
            }
            return path;
        }
        /// <summary>
        /// (递归)去掉所有首部出现的字符串
        /// </summary>
        /// <param name="str">源字符串</param>
        /// <param name="headStr">首部出现的字符串</param>
        /// <returns></returns>
        public static string RemoveHead(string str, string headStr)
        {
            if (str.StartsWith(headStr))
            {
                str = str.Remove(0, headStr.Length);
                return RemoveHead(str, headStr);
            }
            else
            {
                return str;
            }
        }

        /// <summary>  
        /// 返回指定目录下目录信息  
        /// </summary>  
        /// <param name="strDirectory">路径</param>  
        /// <returns></returns>  
        public static DirectoryInfo[] GetDirectory(string strDirectory)
        {
            if (string.IsNullOrEmpty(strDirectory) == false)
            {
                return new DirectoryInfo(strDirectory).GetDirectories();
            }
            return new DirectoryInfo[] { };
        }
        /// <summary>  
        /// 返回指定目录下所有文件信息  
        /// </summary>  
        /// <param name="strDirectory">路径</param>  
        /// <returns></returns>  
        public static FileInfo[] GetFiles(string strDirectory)
        {
            if (string.IsNullOrEmpty(strDirectory) == false)
            {
                return new DirectoryInfo(strDirectory).GetFiles();
            }
            return new FileInfo[] { };
        }
        /// <summary>
        ///  返回指定目录下过滤文件信息  
        /// </summary>
        /// <param name="strDirectory">目录地址</param>
        /// <param name="filter"></param>
        /// <returns></returns>
        public static FileInfo[] GetFiles(string strDirectory, string filter)
        {
            if (string.IsNullOrEmpty(strDirectory) == false)
            {
                return new DirectoryInfo(strDirectory).GetFiles(filter, SearchOption.TopDirectoryOnly);
            }
            return new FileInfo[] { };
        }
        /// <summary>
        /// debug输出
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteDebug(Exception ex)
        {
            Debug.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Debug.WriteLine("Data:" + ex.Data + Environment.NewLine
            + " InnerException:" + ex.InnerException + Environment.NewLine
            + " Message:" + ex.Message + Environment.NewLine
            + " Source:" + ex.Source + Environment.NewLine
            + " StackTrace:" + ex.StackTrace + Environment.NewLine
            + " TargetSite:" + ex.TargetSite);
        }
        /// <summary>
        /// 控制台数据错误
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteConsole(Exception ex)
        {
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            Console.WriteLine("Data:" + ex.Data + Environment.NewLine
            + " InnerException:" + ex.InnerException + Environment.NewLine
            + " Message:" + ex.Message + Environment.NewLine
            + " Source:" + ex.Source + Environment.NewLine
            + " StackTrace:" + ex.StackTrace + Environment.NewLine
            + " TargetSite:" + ex.TargetSite);
        }
        /// <summary>
        /// 错误记录
        /// </summary>
        /// <param name="ex"></param>
        public static void WriteLog(Exception ex)
        {
            WriteLog(ex, null);
        }
        /// <summary>
        /// 错误记录
        /// </summary>
        /// <param name="ex">错误信息</param>
        /// <param name="appendInfo">追加信息</param>
        public static void WriteLog(Exception ex, string appendInfo)
        {
            if (string.IsNullOrEmpty(appendInfo))
            {
                WriteLog("Data:" + ex.Data + Environment.NewLine
            + " InnerException:" + ex.InnerException + Environment.NewLine
            + " Message:" + ex.Message + Environment.NewLine
            + " Source:" + ex.Source + Environment.NewLine
            + " StackTrace:" + ex.StackTrace + Environment.NewLine
            + " TargetSite:" + ex.TargetSite);
            }
            else
            {
                WriteLog("Data:" + ex.Data + Environment.NewLine
            + " InnerException:" + ex.InnerException + Environment.NewLine
            + " Message:" + ex.Message + Environment.NewLine
            + " Source:" + ex.Source + Environment.NewLine
            + " StackTrace:" + ex.StackTrace + Environment.NewLine
            + " TargetSite:" + ex.TargetSite + Environment.NewLine
            + " appendInfo:" + appendInfo);
            }
        }
        /// <summary>
        /// 写log
        /// </summary>
        /// <param name="InfoStr"></param>
        public static void WriteLog(string InfoStr)
        {
            WriteLog(InfoStr, AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "Log");
        }
        /// <summary>
        /// 写log(自动时间log)
        /// </summary>
        /// <param name="InfoStr">内容</param>
        /// <param name="FilePath">目录地址</param>
        public static void WriteLog(string InfoStr, string DirPath)
        {
            WriteLog(InfoStr, DirPath, Encoding.UTF8);
        }
        /// <summary>
        /// 写log(自动时间log)
        /// </summary>
        /// <param name="InfoStr">内容</param>
        /// <param name="DirPath">目录地址</param>
        /// <param name="encoding">编码</param>
        public static void WriteLog(string InfoStr, string DirPath, Encoding encoding)
        {
            FileStream stream = null;
            System.IO.StreamWriter writer = null;
            try
            {
                string logPath = DirPath;
                if (Directory.Exists(logPath) == false)
                {
                    Directory.CreateDirectory(logPath);
                }
                string filepath = logPath + Path.DirectorySeparatorChar + "log_" + DateTime.Now.ToString("yyyyMMddHH") + ".txt";
                if (File.Exists(filepath) == false)
                {
                    stream = new FileStream(filepath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
                }
                else
                {
                    stream = new FileStream(filepath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                }
                writer = new System.IO.StreamWriter(stream, encoding);
                writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                writer.WriteLine(InfoStr);
                writer.WriteLine("");
            }
            finally
            {
                if (writer != null)
                {
                    //writer.Close();
                    writer.Dispose();
                }
                //if (stream != null)
                //{
                //    //stream.Close();
                //    stream.Dispose();
                //}
            }
        }

        /// <summary>
        /// 写log到指定文件（不存在就创建）
        /// </summary>
        /// <param name="InfoStr">内容</param>
        /// <param name="FilePath">文件地址</param>
        public static void WriteLogToFile(string InfoStr, string FilePath)
        {
            WriteLogToFile(InfoStr, FilePath, Encoding.UTF8);
        }
        /// <summary>
        /// 写log到指定文件（不存在就创建）
        /// </summary>
        /// <param name="InfoStr">内容</param>
        /// <param name="FilePath">文件地址</param>
        /// <param name="encoding">编码</param>
        public static void WriteLogToFile(string InfoStr, string FilePath, Encoding encoding)
        {
            FileStream stream = null;
            System.IO.StreamWriter writer = null;
            try
            {
                string logPath = FilePath;
                if (File.Exists(logPath) == false)
                {
                    stream = new FileStream(logPath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
                }
                else
                {
                    stream = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                }

                writer = new System.IO.StreamWriter(stream, encoding);
                writer.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                writer.WriteLine(InfoStr);
                writer.WriteLine("");
            }
            finally
            {
                if (writer != null)
                {
                    //writer.Close();
                    writer.Dispose();
                }
                //if (stream != null)
                //{
                //    //stream.Close();
                //    stream.Dispose();
                //}
            }
        }
        /// <summary>
        /// 写内容到指定文件（不存在就创建）
        /// </summary>
        /// <param name="InfoStr">内容</param>
        /// <param name="FilePath">文件地址</param>
        /// <param name="encoding">编码</param>
        /// <param name="append">是否追加</param>
        public static void WriteInfoToFile(string InfoStr, string FilePath, Encoding encoding, bool append)
        {
            FileStream stream = null;
            System.IO.StreamWriter writer = null;
            try
            {
                string logPath = FilePath;
                if (File.Exists(logPath) == false)
                {
                    stream = new FileStream(logPath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
                    writer = new System.IO.StreamWriter(stream, encoding);
                }
                else
                {
                    //存在就覆盖
                    writer = new System.IO.StreamWriter(logPath, append, encoding);
                }
                writer.Write(InfoStr);
            }
            finally
            {
                if (writer != null)
                {
                    //writer.Close();
                    writer.Dispose();
                }
                //if (stream != null)
                //{
                //    //stream.Close();
                //    stream.Dispose();
                //}
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="InfoStr"></param>
        /// <param name="FilePath"></param>
        /// <param name="encoding"></param>
        public static void WriteInfoToFile(string InfoStr, string FilePath, Encoding encoding)
        {
            WriteInfoToFile(InfoStr, FilePath, encoding, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="InfoStr"></param>
        /// <param name="FilePath"></param>
        public static void WriteInfoToFile(string InfoStr, string FilePath)
        {
            WriteInfoToFile(InfoStr, FilePath, Encoding.UTF8, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="datagram"></param>
        /// <param name="FilePath"></param>
        /// <param name="callback"></param>
        /// <param name="numBytes"></param>
        public static void AsyncWriteLog(byte[] datagram, string FilePath, AsyncCallback callback, int numBytes)
        {
            FileStream stream = null;
            try
            {
                string logPath = FilePath;
                if (File.Exists(logPath) == false)
                {
                    stream = new FileStream(logPath, FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite);
                }
                else
                {
                    stream = new FileStream(logPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                }

                if (stream.CanWrite)
                {

                    stream.BeginWrite(datagram, 0, numBytes, callback, "AsyncWriteLog_" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                }
                else
                {
                    throw new Exception("文件无法写入，文件或只读！");
                }
            }
            finally
            {
                if (stream != null)
                {
                    //stream.Close();
                    stream.Dispose();
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="datagram"></param>
        /// <param name="FilePath"></param>
        /// <param name="numBytes"></param>
        public static void AsyncWriteLog(byte[] datagram, string FilePath, int numBytes)
        {
            AsyncWriteLog(datagram, FilePath, new AsyncCallback(AsyncCallbackFunc), numBytes);
        }
        public static void AsyncCallbackFunc(IAsyncResult result)
        {
            FileStream filestream = result.AsyncState as FileStream;
            filestream.EndWrite(result);
            filestream.Close();
        }

        /// <summary>
        /// 文本转义（方便讲文本转换成C#变量代码）
        /// 例子 " 转化成 string str="\"";
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string ToEscape(string str, string m_var)
        {
            /*
                "           \"
                \           \\
            */
            str = str.Trim();
            str = str.Replace("\\", "\\\\");
            str = str.Replace("\"", "\\\"");
            return "string " + m_var + "=\"" + str + "\";";
        }
        public static string ReadTxt(string filePath)
        {
            return ReadTxt(filePath, Encoding.UTF8);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static string ReadTxt(string filePath, Encoding enc)
        {
            FileStream stream = null;
            System.IO.StreamReader reader = null;
            string result = "";
            try
            {
                if (File.Exists(filePath) == false)
                {
                    stream = new FileStream(filePath, FileMode.CreateNew, FileAccess.Read, FileShare.ReadWrite);
                }
                else
                {
                    stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    reader = new System.IO.StreamReader(stream, enc);
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                WriteLog(e);
            }
            finally
            {
                if (reader != null)
                {
                    //reader.Close();
                    reader.Dispose();
                }
                //if (stream != null)
                //{
                //    //stream.Close();
                //    stream.Dispose();
                //}
            }
            return result;
        }
        /// <summary>
        /// 读取Appliction目录下的文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="enc"></param>
        /// <returns></returns>
        public static string AppReadTxt(string filePath, Encoding enc)
        {

            string appPath = AppDomain.CurrentDomain.BaseDirectory;
            string path = appPath + filePath;
            return ReadTxt(path, enc);
        }
        /// <summary>
        /// 读取Appliction目录下的文件（UTF-8）
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string AppReadTxt(string filePath)
        {
            return AppReadTxt(filePath, System.Text.Encoding.UTF8);
        }
    }
}
