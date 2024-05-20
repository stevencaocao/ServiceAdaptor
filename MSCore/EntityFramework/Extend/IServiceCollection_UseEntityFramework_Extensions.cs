using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MSCore.EntityFramework.DbContextInitor;
using MSCore.EntityFramework.Model;
using MSCore.Util.ConfigurationManager;

namespace MSCore.EntityFramework
{
    public static partial class IServiceCollection_UseEntityFramework_Extensions
    {
        /// <summary>
        /// 启用EntityFramework
        /// 使用appsettings.json中的配置
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="data"></param>
        /// <param name="configPath">在appsettings.json中的路径，默认："App.Db"</param>
        public static bool UseMSCoreEFCore<TContext>(this IServiceCollection data, string configPath = "App.Db.Project") where TContext : DbContext
        {
            var cInfo = Appsettings.json.GetByPath<ConnectionInfoPlus>(configPath ?? "App.Db.Project");
            cInfo.ConnectionKey = configPath;
            Appsettings.DatabaseType = cInfo.type;
            if (cInfo.type == "dm")
            {
                string schema = cInfo.ConnectionString.Substring(cInfo.ConnectionString.ToLower().IndexOf("database")).Split(';')[0].Split('=')[1];
                Appsettings.DatabasePrefix = schema + ".";
            }
            return UseEntityFramework<TContext>(data, cInfo);
        }


        /// <summary>
        /// 启用EntityFramework
        /// </summary>
        /// <typeparam name="TContext"></typeparam>
        /// <param name="data"></param>
        /// <param name="info">如：{"type":"mysql","ConnectionString":"xxx"}</param>
        private static bool UseEntityFramework<TContext>(this IServiceCollection data, ConnectionInfoPlus info) where TContext : DbContext
        {
            if (!DbContextInitors.DbContextInitorMap.TryGetValue(info.type, out var initor) || initor == null) return false;
            initor.AddDbContext<TContext>(data, info);

            return true;
        }
    }
}
