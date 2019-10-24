using System.Collections.Generic;

namespace WeihanLi.Common.DependencyInjection
{
    internal class ServiceFactory
    {
        private readonly IReadOnlyCollection<ServiceDefinition> _serviceDefinitions;

        public ServiceFactory(IReadOnlyCollection<ServiceDefinition> serviceDefinitions)
        {
            _serviceDefinitions = serviceDefinitions;
        }
    }
}
