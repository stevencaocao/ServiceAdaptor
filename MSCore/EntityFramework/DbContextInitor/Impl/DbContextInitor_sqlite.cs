using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MSCore.EntityFramework.Model;

namespace MSCore.EntityFramework.DbContextInitor
{
    public class DbContextInitor_sqlite : IDbContextInitor
    {
        public void AddDbContext<TContext>(IServiceCollection data, ConnectionInfoPlus info) where TContext : DbContext
        {
            //使用sqlite数据库
            data.AddDbContext<TContext>(opt => opt.UseSqlite(info.ConnectionString));
        }
    }

}
