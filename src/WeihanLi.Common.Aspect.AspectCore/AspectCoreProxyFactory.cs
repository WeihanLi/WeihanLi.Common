using System;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Aspect.AspectCore
{
    internal sealed class AspectCoreProxyFactory : IProxyFactory
    {
        public static readonly IProxyFactory Instance = new AspectCoreProxyFactory(DependencyResolver.Current);

        private readonly IServiceProvider _serviceProvider;

        public AspectCoreProxyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object CreateProxy(Type serviceType)
        {
            if (null == serviceType)
                throw new ArgumentNullException(nameof(serviceType));

            if (serviceType.IsInterface)
                return AspectCoreHelper.ProxyGenerator.CreateInterfaceProxy(serviceType);

            var ctorArguments = ActivatorHelper.GetBestConstructorArguments(_serviceProvider, serviceType);
            return AspectCoreHelper.ProxyGenerator.CreateClassProxy(serviceType, serviceType, ctorArguments);
        }

        public object CreateProxy(Type serviceType, Type implementType)
        {
            if (null == serviceType)
                throw new ArgumentNullException(nameof(serviceType));
            if (null == implementType)
                throw new ArgumentNullException(nameof(implementType));

            if (serviceType.IsInterface)
                return AspectCoreHelper.ProxyGenerator.CreateInterfaceProxy(serviceType, _serviceProvider.CreateInstance(implementType));

            var ctorArguments = ActivatorHelper.GetBestConstructorArguments(_serviceProvider, implementType);
            return AspectCoreHelper.ProxyGenerator.CreateClassProxy(serviceType, implementType, ctorArguments);
        }

        public object CreateProxyWithTarget(Type serviceType, object implement)
        {
            if (null == serviceType)
                throw new ArgumentNullException(nameof(serviceType));
            if (null == implement)
                throw new ArgumentNullException(nameof(implement));

            if (serviceType.IsInterface)
                return AspectCoreHelper.ProxyGenerator.CreateInterfaceProxy(serviceType, implement);

            return AspectCoreHelper.ProxyGenerator.CreateClassProxy(serviceType, implement.GetType(), ArrayHelper.Empty<object>());
        }
    }
}
