using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BIMProduct.BE.BASE
{
    public interface IUnitOfWork : IDisposable
    {
        IUnitOfWork BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();
    }
}
