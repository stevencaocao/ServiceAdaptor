using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MSCore.EntityFramework.Model;

namespace MSCore.EntityFramework.DbContextInitor
{
    public interface IDbContextInitor
    {
        void AddDbContext<TContext>(IServiceCollection data, ConnectionInfoPlus info) where TContext : DbContext;
    }

}
