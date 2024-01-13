using System.Data;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Data;

[CLSCompliant(false)]
public class UnitOfWork : IUnitOfWork, IDisposable
{
    private readonly IDbTransaction _dbTransaction;

    protected IDbTransaction DbTransaction => _dbTransaction;

    public UnitOfWork(IDbConnection dbConnection)
    {
        Guard.NotNull(dbConnection);
        dbConnection.EnsureOpen();
        _dbTransaction = dbConnection.BeginTransaction();
    }

    public UnitOfWork(IDbTransaction dbTransaction)
    {
        _dbTransaction = Guard.NotNull(dbTransaction);
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
