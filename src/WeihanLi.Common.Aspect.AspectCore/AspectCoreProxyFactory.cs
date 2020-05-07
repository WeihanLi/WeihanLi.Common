using System;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Aspect.AspectCore
{
    internal sealed class AspectCoreProxyFactory : IProxyFactory
    {
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

            return AspectCoreHelper.ProxyGenerator.CreateClassProxy(serviceType, serviceType, ArrayHelper.Empty<object>());
        }

        public object CreateProxy(Type serviceType, Type implementType)
        {
            if (null == serviceType)
                throw new ArgumentNullException(nameof(serviceType));
            if (null == implementType)
                throw new ArgumentNullException(nameof(implementType));

            if (serviceType.IsInterface)
                return AspectCoreHelper.ProxyGenerator.CreateInterfaceProxy(serviceType, _serviceProvider.CreateInstance(implementType));

            return AspectCoreHelper.ProxyGenerator.CreateClassProxy(serviceType, implementType, ArrayHelper.Empty<object>());
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
