using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace BIMProduct.BE.BASE
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
        int ExcuteSql(string strSql, params SqlParameter[] parameters);
        bool Update(List<TEntity> entityToUpdate);


        void ChangeTable(string table);

        int Save();
        int SaveChangesCount();
    }
}
