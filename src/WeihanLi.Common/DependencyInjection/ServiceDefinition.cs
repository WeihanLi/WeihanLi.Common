using System;

namespace WeihanLi.Common.DependencyInjection
{
    public class ServiceDefinition
    {
        public ServiceLifetime Lifetime { get; }

        public Type ImplementType { get; }

        public Type ServiceType { get; }

        public object ImplementationInstance { get; }

        public Func<IServiceProvider, object> ImplementationFactory { get; }
    }
}
