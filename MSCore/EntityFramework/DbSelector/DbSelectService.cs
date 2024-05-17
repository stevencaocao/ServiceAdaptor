using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using MSCore.Util.Threading.Cache;
using System.Collections.Generic;

namespace MSCore.EntityFramework.DbSelector
{
    public static class DbSelectService
    {
        //Primary   project
        //Standby   project
        /// <summary>
        /// 主机链接字符串
        /// </summary>
        public readonly static AsyncCache<Dictionary<object, string>> PrimaryConnectionStringMap = new AsyncCache<Dictionary<object, string>>();
        /// <summary>
        /// 备机链接字符串
        /// </summary>
        public readonly static AsyncCache<Dictionary<object, string>> StandbyConnectionStringMap = new AsyncCache<Dictionary<object, string>>();


        public static void LoadDBContext<T>(this T data, string method) where T : DbContext
        {
            string connectionKey = data.GetType().GetProperty("ConnectionKey").GetValue(data, null)?.ToString() ?? "App.Db.Project";

            var dbConnection = data.Database.GetDbConnection();
            var connectionMap = PrimaryConnectionStringMap.Value;

            if (connectionMap == null)
            {
                PrimaryConnectionStringMap.Value = connectionMap = new Dictionary<object, string>();
            }

            if (!connectionMap.ContainsKey(connectionKey))
            {
                connectionMap[connectionKey] = dbConnection.ConnectionString;
            }

            dbConnection.ConnectionString = connectionMap[connectionKey];
        }


        /// <summary>
        /// 使用数据库主备切换中间件
        /// </summary>
        /// <typeparam name="DbContext"></typeparam>
        /// <param name="builder"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseDbSelector<DbContext>(this IApplicationBuilder builder) where DbContext : Microsoft.EntityFrameworkCore.DbContext
        {
            return builder.UseMiddleware<DbSelectMiddleware<DbContext>>();
        }
    }
}
