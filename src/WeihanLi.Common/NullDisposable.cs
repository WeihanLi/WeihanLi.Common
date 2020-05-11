using System;
using System.Threading;

namespace WeihanLi.Common
{
    /// <summary>
    /// A singleton disposable that does nothing when disposed.
    /// </summary>
    public sealed class NullDisposable : IDisposable
    {
        private NullDisposable()
        {
        }

        public void Dispose()
        {
        }

        /// <summary>
        /// Gets the instance of <see cref="NullDisposable"/>.
        /// </summary>
        public static NullDisposable Instance { get; } = new NullDisposable();
    }

    public sealed class DisposableAction : IDisposable
    {
        public static readonly DisposableAction Empty = new DisposableAction(null);

        private Action _disposeAction;

        public DisposableAction(Action disposeAction)
        {
            _disposeAction = disposeAction;
        }

        public void Dispose()
        {
            Interlocked.Exchange(ref _disposeAction, null)?.Invoke();
        }
    }
}
