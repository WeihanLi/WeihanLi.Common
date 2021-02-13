using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        protected readonly IDbTransaction _dbTransaction;

        public UnitOfWork(IDbConnection dbConnection)
        {
            if (null == dbConnection)
            {
                throw new ArgumentNullException(nameof(dbConnection));
            }
            dbConnection.EnsureOpen();
            _dbTransaction = dbConnection.BeginTransaction();
        }

        public UnitOfWork(IDbTransaction dbTransaction)
        {
            _dbTransaction = dbTransaction ?? throw new ArgumentNullException(nameof(dbTransaction));
        }

        public virtual void Commit() => _dbTransaction.Commit();

        public virtual Task CommitAsync(CancellationToken cancellationToken = default)
        {
            _dbTransaction.Commit();
            return Task.CompletedTask;
        }

        public virtual void Rollback() => _dbTransaction.Rollback();

        public virtual Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            _dbTransaction.Rollback();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Cleanup
                _dbTransaction.Dispose();
            }
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
