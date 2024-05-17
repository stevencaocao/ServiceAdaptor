using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace BIMProduct.BE.BASE
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        protected DBContext Context;
        protected DbSet<TEntity> DbSet;

        public Repository(DBContext context)
        {
            this.Context = context;
            this.DbSet = context.Set<TEntity>();
        }

        /// <summary>
        /// 获取一个对象集合
        /// </summary>
        /// <param name="filter">过滤条件</param>
        /// <param name="orderBy">排序条件</param>
        /// <param name="includeProperties">要查询的字段(以,拼接)</param>
        /// <returns></returns>
        public virtual IQueryable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = this.DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                return orderBy(query);
            }
            else
            {
                return query;
            }
        }

        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="strsql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public virtual int ExcuteSql(string strsql, params DbParameter[] parameters)
        {
            return Context.Database.ExecuteSqlCommand(strsql, parameters);
        }

        /// <summary>
        /// 根据id获取某个实体对象 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity GetByID(object id)
        {
            return DbSet.Find(id);
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual TEntity Insert(TEntity entity)
        {
            TEntity addEntity = DbSet.Add(entity).Entity;
            return addEntity;
        }

        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public virtual void Insert(List<TEntity> entities)
        {
            DbSet.AddRange(entities);
        }

        /// <summary>
        /// 根据id删除某个对象
        /// </summary>
        /// <param name="id">对象id</param>
        public virtual void Delete(object id)
        {
            TEntity entityToDelete = DbSet.Find(id);
            if (entityToDelete != null)
                Delete(entityToDelete);
        }

        /// <summary>
        /// 删除某个实体
        /// </summary>
        /// <param name="entityToDelete">实体对象</param>
        public virtual void Delete(TEntity entityToDelete)
        {
            if (Context.Entry(entityToDelete).State == EntityState.Detached)
            {
                DbSet.Attach(entityToDelete);
            }
            DbSet.Remove(entityToDelete);
        }

        /// <summary>
        /// 根据某个条件删除某个对象
        /// </summary>
        /// <param name="predicate">lamda表达式查询条件</param>
        /// <returns></returns>
        public virtual int Delete(Expression<Func<TEntity, bool>> predicate)
        {
            var entitys = DbSet.Where(predicate).ToList();
            entitys.ForEach(m => Context.Entry<TEntity>(m).State = EntityState.Deleted);
            return Context.SaveChanges();
        }

        /// <summary>
        /// 更新某个实体对象
        /// </summary>
        /// <param name="entityToUpdate">实体对象</param>
        public virtual bool Update(TEntity entityToUpdate)
        {
            if (Context.Entry(entityToUpdate).State != EntityState.Detached)
                //      Context.Entry<TEntity>(entityToUpdate).State = EntityState;
                DbSet.Attach(entityToUpdate);
            //获取当前的所有的属性 判断属性值是不是 null 如果是null 那么就不进行更新改字段
            PropertyInfo[] properties = entityToUpdate.GetType().GetProperties();
            // Context.Entry(entityToUpdate).State = EntityState.Modified;
            foreach (PropertyInfo prop in properties)
            {
                if ((prop.GetValue(entityToUpdate, null) != null))
                {
                    if (prop.GetValue(entityToUpdate, null).ToString() == "&nbsp;")
                        Context.Entry(entityToUpdate).Property(prop.Name).CurrentValue = null;
                    if (!Context.Entry(entityToUpdate).Property(prop.Name).Metadata.IsPrimaryKey()) //主键是不能进行修改的 否则无法进行更新
                    {
                        //数字传0不更新与巡检计划中取消派单方法冲突。（取消要将字段receiver重置为0）
                        //if (prop.GetValue(entityToUpdate, null).ToString() == "0" && prop.PropertyType == typeof(int)) //数字传0也不更新
                            //{
                            //    Context.Entry(entityToUpdate).Property(prop.Name).IsModified = false;
                            //}
                            //      else
                            //{
                            Context.Entry(entityToUpdate).Property(prop.Name).IsModified = true;
                        //}
                    }
                }
            }
            return this.Context.SaveChanges() > 0;
            //Context.Entry(entityToUpdate).State = EntityState.Modified;
        }
        /// <summary>
        /// 批量编辑
        /// </summary>
        /// <param name="entitiesUpdate"></param>
        /// <returns></returns>
        public virtual bool Update(List<TEntity> entitiesUpdate)
        {
            entitiesUpdate.ForEach(m => Context.Entry<TEntity>(m).State = EntityState.Modified);
            return Context.SaveChanges() > 0;
        }



        public void ChangeTable(string table)
        {
            if (Context.Model.FindEntityType(typeof(TEntity)).Relational() is RelationalEntityTypeAnnotations relational)
            {
                relational.TableName = table;
            }
        }



        public int Save()
        {
            return this.Context.SaveChanges();
        }

        public int SaveChangesCount()
        {
            return this.Context.SaveChanges();
        }
    }
}
