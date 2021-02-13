using System;
using System.Threading;
using System.Threading.Tasks;

namespace WeihanLi.Common
{
    /// <summary>
    /// AsyncLock basedOn SemaphoreSlim
    /// </summary>
    public sealed class AsyncLock : IDisposable
    {
        private readonly SemaphoreSlim _mutex = new(1, 1);

        public IDisposable Lock()
        {
            _mutex.Wait();
            return new AsyncLockReleaser(_mutex);
        }

        public Task<IDisposable> LockAsync() => LockAsync(CancellationToken.None);

        public Task<IDisposable> LockAsync(CancellationToken cancellationToken) => LockAsync(TimeSpan.Zero, cancellationToken);

        public async Task<IDisposable> LockAsync(TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (timeout <= TimeSpan.Zero)
            {
                await _mutex.WaitAsync(cancellationToken);
            }
            else
            {
                await _mutex.WaitAsync(timeout, cancellationToken);
            }
            return new AsyncLockReleaser(_mutex);
        }

        public void Dispose()
        {
            _mutex.Dispose();
        }

        #region AsyncLockReleaser

        private struct AsyncLockReleaser : IDisposable
        {
            private readonly SemaphoreSlim _semaphoreSlim;

            public AsyncLockReleaser(SemaphoreSlim semaphoreSlim) => _semaphoreSlim = semaphoreSlim;

            public void Dispose()
            {
                _semaphoreSlim.Release();
            }
        }

        #endregion AsyncLockReleaser
    }
}
