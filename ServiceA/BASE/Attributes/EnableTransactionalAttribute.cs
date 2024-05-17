using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace ServiceA.BASE
{
    /// <summary>
    /// 开启事务处理
    /// </summary>
    public class EnableTransactionalAttribute : Attribute
    {
        private IDbContextTransaction? _transaction;

       

        public async Task<TResult> ExecuteInTransaction<TDbContext, TResult>(
            TDbContext dbContext, Func<Task<TResult>> operation)
            where TDbContext : DbContext
        {
            _transaction = dbContext.Database.BeginTransaction();

            try
            {
                var result = await operation();
                await _transaction.CommitAsync();
                return result;
            }
            catch (Exception)
            {
                await RollbackTransaction();
                throw;
            }
            finally
            {
                await DisposeTransaction();
            }
        }

        private async Task RollbackTransaction()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
            }
        }

        private async Task DisposeTransaction()
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
            }
        }
    }
}
