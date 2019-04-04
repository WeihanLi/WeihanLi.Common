using System;
using System.Threading;
using System.Threading.Tasks;

namespace WeihanLi.Common
{
    /// <summary>
    /// 基于 SemaphoreSlim 的 异步锁
    /// </summary>
    public sealed class AsyncLock : IDisposable
    {
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

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

        #region IDisposable Support

        private bool _disposed; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // 释放托管状态(托管对象)。
                }

                _mutex.Dispose();
                _disposed = true;
            }
        }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support

        #region AsyncLockReleaser

        private struct AsyncLockReleaser : IDisposable
        {
            private readonly SemaphoreSlim _semaphoreSlim;

            public AsyncLockReleaser(SemaphoreSlim semaphoreSlim) => _semaphoreSlim = semaphoreSlim;

            public void Dispose()
            {
                _semaphoreSlim?.Release();
            }
        }

        #endregion AsyncLockReleaser
    }
}
