using System;

namespace WeihanLi.Common
{
    public interface IScope : IDisposable
    {
    }

    public class NullScope : IScope
    {
        public void Dispose()
        {
        }

        public static NullScope Instance { get; } = new();
    }
}
