using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MSCore.EntityFramework.DbSelector;
using MSCore.EntityFramework.Model;

namespace MSCore.EntityFramework.DbContextInitor
{
    /// <summary>
    /// 达梦数据库支持
    /// </summary>
    public class DbContextInitor_dm : IDbContextInitor
    {
        public void AddDbContext<TContext>(IServiceCollection data, ConnectionInfoPlus info) where TContext : DbContext
        {

            // for Microsoft.EntityFrameworkCore.Dm
            data.AddDbContext<TContext>(opt => opt.UseDm(info.ConnectionString));

            var primaryConnectionMap = DbSelectService.PrimaryConnectionStringMap.Value;

            if (primaryConnectionMap == null)
            {
                DbSelectService.PrimaryConnectionStringMap.Value = primaryConnectionMap = new System.Collections.Generic.Dictionary<object, string>();
            }

            if (!primaryConnectionMap.ContainsKey(info.ConnectionKey))
            {
                primaryConnectionMap[info.ConnectionKey] = info.ConnectionString;
            }

            var standbyConnectionMap = DbSelectService.StandbyConnectionStringMap.Value;

            if (standbyConnectionMap == null)
            {
                DbSelectService.StandbyConnectionStringMap.Value = standbyConnectionMap = new System.Collections.Generic.Dictionary<object, string>();
            }

            if (!standbyConnectionMap.ContainsKey(info.ConnectionKey))
            {
                standbyConnectionMap[info.ConnectionKey] = info.ConnectionStringStandby;
            }

        }
    }
}
