using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WeihanLi.Common.DependencyInjection
{
    public interface IServiceContainerBuilder : IEnumerable<ServiceDefinition>
    {
        IServiceContainerBuilder Add(ServiceDefinition item);

        IServiceContainerBuilder TryAdd(ServiceDefinition item);

        IServiceContainer Build();
    }

    public sealed class ServiceContainerBuilder : IServiceContainerBuilder
    {
        private readonly List<ServiceDefinition> _services = new List<ServiceDefinition>();

        public IServiceContainerBuilder Add(ServiceDefinition item)
        {
            if (_services.Any(_ => _.ServiceType == item.ServiceType && _.GetImplementType() == item.GetImplementType()))
            {
                return this;
            }

            _services.Add(item);
            return this;
        }

        public IServiceContainerBuilder TryAdd(ServiceDefinition item)
        {
            if (_services.Any(_ => _.ServiceType == item.ServiceType))
            {
                return this;
            }
            _services.Add(item);
            return this;
        }

        public IServiceContainer Build() => new ServiceContainer(_services);

        public IEnumerator<ServiceDefinition> GetEnumerator() => _services.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
