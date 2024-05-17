using System.Data.Common;
using System.Linq.Expressions;

namespace ServiceA.BASE
{
    public interface IRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        TEntity GetByID(object id);

        TEntity Insert(TEntity entity);

        void Insert(List<TEntity> entities);

        void Delete(object id);

        void Delete(TEntity entityToDelete);

        int Delete(Expression<Func<TEntity, bool>> predicate);

        bool Update(TEntity entityToUpdate);
        int ExcuteSql(string strSql, params DbParameter[] parameters);
        bool Update(List<TEntity> entityToUpdate);

        int Save();
        int SaveChangesCount();
    }
}
