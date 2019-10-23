using System;
using System.Linq;

namespace WeihanLi.Common.DependencyInjection
{
    internal class ServiceContainerServiceProvider : IServiceProvider
    {
        private readonly ServiceContainer _serviceContainer;

        public ServiceContainerServiceProvider(ServiceContainer serviceContainer)
        {
            _serviceContainer = serviceContainer;
        }

        public object GetService(Type serviceType)
        {
            //if (serviceType.IsAssignableFrom(typeof(IEnumerable<>)))
            //{
            //    _serviceContainer.services.
            //}

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
