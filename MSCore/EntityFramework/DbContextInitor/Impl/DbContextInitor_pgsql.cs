using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MSCore.EntityFramework.Model;
using System;

namespace MSCore.EntityFramework.DbContextInitor
{
    public class DbContextInitor_pgsql : IDbContextInitor
    {
        public void AddDbContext<TContext>(IServiceCollection data, ConnectionInfoPlus info) where TContext : DbContext
        {
            //使用PostgreSQL数据库

            // for Npgsql.EntityFrameworkCore.PostgreSQL
            data.AddDbContext<TContext>(opt => opt.UseNpgsql(info.ConnectionString));
        }
    }
}
