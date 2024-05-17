namespace MSCore.EntityFramework.Model
{
    /// <summary>
    /// 添加备库链接字符串
    /// </summary>
    public class ConnectionInfoPlus : ConnectionInfo
    {
        /// <summary>
        /// Key
        /// </summary>
        public string ConnectionKey { get; set; }
        /// <summary>
        /// 备库数据库连接字符串
        /// </summary>
        public string ConnectionStringStandby { get; set; }

    }
}
