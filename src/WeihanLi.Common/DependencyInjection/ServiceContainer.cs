using System;
using System.Collections.Generic;

namespace WeihanLi.Common.DependencyInjection
{
    public interface IServiceContainer : IScope, IServiceProvider
    {
        void Add(ServiceDefinition item);

        void Clear();

        IServiceContainer CreateScope();
    }

    public class ServiceContainer : IServiceContainer
    {
        private readonly List<ServiceDefinition> _services = new List<ServiceDefinition>();

        public void Add(ServiceDefinition item)
        {
            _services.Add(item);
        }

        public void Clear()
        {
            _services.Clear();
        }

        public IServiceContainer CreateScope()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public object GetService(Type serviceType)
        {
            throw new NotImplementedException();
        }
    }
}
