using System;
using System.Collections.Generic;
using System.Linq;

namespace WeihanLi.Common.DependencyInjection
{
    internal class ServiceContainerServiceProvider : IServiceProvider
    {
        private readonly IReadOnlyCollection<ServiceDefinition> _serviceContainer;

        public ServiceContainerServiceProvider(IReadOnlyCollection<ServiceDefinition> serviceContainer)
        {
            _serviceContainer = serviceContainer;
        }

        public object GetService(Type serviceType)
        {
            if (typeof(IEnumerable<>).MakeGenericType(typeof(object)).IsAssignableFrom(serviceType))
            {
                // get service collection
                var services = _serviceContainer.Where(_ => _.ServiceType == serviceType).ToArray();

                return services.Select(s => s.ImplementationInstance).ToArray();
            }
            else
            {
                var service = _serviceContainer.LastOrDefault(s => s.ServiceType == serviceType);
                if (null == service)
                {
                    throw new InvalidOperationException($"service {serviceType.FullName} had not been registered");
                }
                // 由 serviceFactory 去创建一个实例
                return service.ImplementationInstance;
            }
        }
    }
}
