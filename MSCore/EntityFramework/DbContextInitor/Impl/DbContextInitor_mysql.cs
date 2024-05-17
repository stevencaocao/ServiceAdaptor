using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MSCore.EntityFramework.Model;

namespace MSCore.EntityFramework.DbContextInitor
{
    public partial class DbContextInitor_mysql : IDbContextInitor
    {
        public void AddDbContext<TContext>(IServiceCollection data, ConnectionInfoPlus info) where TContext : DbContext
        {
            //使用mysql数据库

            // for MySql.EntityFrameworkCore
            data.AddDbContext<TContext>(opt => opt.UseMySQL(info.ConnectionString));
        }
    }
}
