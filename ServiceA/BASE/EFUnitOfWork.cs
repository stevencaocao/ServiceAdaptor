using Microsoft.EntityFrameworkCore.Storage;
using MSCore.EntityFramework;
using System.Data;
using System.Data.Common;

namespace ServiceA.BASE
{
    /// <summary>
    /// 用于创建唯一上下文 保证线程内唯一
    /// </summary>
    public class EFUnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DBContext _context;
        private DbTransaction dbTransaction = null;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context">startup时创建的Context</param>
        public EFUnitOfWork(DBContext context)
        {
            _context = context;
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="context">startup时创建的Context</param>
        public EFUnitOfWork(DBContext context,IsolationLevel isolationLevel)
        {
            _context = context;
        }


        /// <summary>
        /// 执行事务
        /// </summary>
        /// <returns></returns>
        public IUnitOfWork BeginTransaction()
        {
            _context.Database.BeginTransaction();
            dbTransaction = _context.Database.CurrentTransaction.GetDbTransaction();
            return this;
        }

        /// <summary>
        /// 提交事务
        /// </summary>
        public void CommitTransaction()
        {
            _context.SaveChanges();
            _context.Database.CommitTransaction();
        }


        /// <summary>
        /// 释放
        /// </summary>
        public void Dispose()
        {
            if (dbTransaction != null)
            {
                this.dbTransaction.Dispose();
            }

            this._context.Dispose();
        }

        /// <summary>
        /// 事务回滚
        /// </summary>
        public void RollbackTransaction()
        {
            _context.Database.RollbackTransaction();
        }
    }
}