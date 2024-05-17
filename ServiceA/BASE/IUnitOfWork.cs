namespace ServiceA.BASE
{
    public interface IUnitOfWork : IDisposable
    {
        IUnitOfWork BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();
    }
}
