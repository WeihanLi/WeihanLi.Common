using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Data
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        protected readonly DbTransaction _dbTransaction;

        public UnitOfWork(DbConnection dbConnection)
        {
            if (null == dbConnection)
            {
                throw new ArgumentNullException(nameof(dbConnection));
            }
            dbConnection.EnsureOpen();
            _dbTransaction = dbConnection.BeginTransaction();
        }

        public UnitOfWork(DbTransaction dbTransaction)
        {
            _dbTransaction = dbTransaction ?? throw new ArgumentNullException(nameof(dbTransaction));
        }

        public void Commit() => _dbTransaction.Commit();

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            _dbTransaction.Commit();
            return TaskHelper.CompletedTask;
        }

        public void Rollback() => _dbTransaction.Rollback();

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            _dbTransaction.Rollback();
            return TaskHelper.CompletedTask;
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
                _dbTransaction?.Dispose();
            }
        }

        ~UnitOfWork()
        {
            Dispose(false);
        }
    }
}
