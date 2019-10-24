using System;

namespace WeihanLi.Common.DependencyInjection
{
    public interface IServiceScope : IScope
    {
        IServiceProvider ServiceProvider { get; }
    }

    internal class ServiceScope : IServiceScope
    {
        public void Dispose()
        {
            // dispose scope services
        }

        public IServiceProvider ServiceProvider { get; }
    }
}
