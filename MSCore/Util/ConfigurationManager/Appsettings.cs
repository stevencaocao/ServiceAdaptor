using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace MSCore.Util.ConfigurationManager
{
    public class Appsettings : JsonFile
    {
        #region json
        private static JsonFile _json;
        /// <summary>
        /// get config from appsettings.json
        /// </summary>
        public static JsonFile json
        {
            get { return _json ?? (_json = new Appsettings()); }
            set { _json = value; }
        }
        #endregion

        /// <summary>
        /// 达梦数据库，查询表增加数据库前缀（通过sql直接查询时需要）
        /// </summary>
        public static string DatabasePrefix = "";

        /// <summary>
        /// 数据库类型，mysql,mssql,dm,sqlite
        /// </summary>
        public static string DatabaseType = "";



        const string fileName = "appsettings.json";

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected static string GetDefaultPath()
        {
            return fileName;
        }


        private Appsettings(string configPath = null) : base(configPath ?? GetDefaultPath())
        {
        }

        public override void SaveToFile()
        {
            throw new Exception("[ConfigurationManager] not allowed to Save to " + fileName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string AbsPath(string path = "")
        {
            return Path.Combine(AppContext.BaseDirectory, path);
        }


    }
}
