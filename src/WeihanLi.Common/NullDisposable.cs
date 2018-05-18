using System;

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
}
