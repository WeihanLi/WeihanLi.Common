using System.Threading;
using System.Threading.Tasks;

namespace WeihanLi.Common.Data;

public interface IUnitOfWork
{
    void Commit();

    Task CommitAsync(CancellationToken cancellationToken = default);

    void Rollback();

    Task RollbackAsync(CancellationToken cancellationToken = default);
}
