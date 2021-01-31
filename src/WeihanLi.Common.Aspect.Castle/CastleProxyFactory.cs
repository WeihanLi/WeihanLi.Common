using System;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Aspect.Castle
{
    internal sealed class CastleProxyFactory : IProxyFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public static readonly IProxyFactory Instance = new CastleProxyFactory(DependencyResolver.Current);

        public CastleProxyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object CreateProxy(Type serviceType, object?[] arguments)
        {
            if (null == serviceType)
                throw new ArgumentNullException(nameof(serviceType));

            if (serviceType.IsInterface)
            {
                return CastleHelper.ProxyGenerator.CreateInterfaceProxyWithoutTarget(serviceType,
                        new CastleFluentAspectInterceptor());
            }

            var ctorArguments = ActivatorHelper.GetBestConstructorArguments(_serviceProvider, serviceType, arguments);
            return CastleHelper.ProxyGenerator.CreateClassProxy(
                serviceType, ctorArguments, new CastleFluentAspectInterceptor()
                );
        }

        public object CreateProxy(Type serviceType, Type implementType, params object?[] arguments)
        {
            if (null == serviceType)
                throw new ArgumentNullException(nameof(serviceType));

            if (null == implementType)
                throw new ArgumentNullException(nameof(implementType));

            if (serviceType.IsInterface)
            {
                var target = _serviceProvider.CreateInstance(implementType, arguments);
                return CreateProxyWithTarget(serviceType, target, Array.Empty<object>());
            }

            return CreateProxy(implementType, arguments);
        }

        public object CreateProxyWithTarget(Type serviceType, object implement, object?[] arguments)
        {
            if (null == serviceType)
                throw new ArgumentNullException(nameof(serviceType));

            if (null == implement)
                throw new ArgumentNullException(nameof(implement));

            if (serviceType.IsInterface)
            {
                return CastleHelper.ProxyGenerator.CreateInterfaceProxyWithTarget(serviceType, implement,
                    new CastleFluentAspectInterceptor());
            }
            return CastleHelper.ProxyGenerator.CreateClassProxyWithTarget(serviceType, implement, arguments,
                new CastleFluentAspectInterceptor());
        }
    }
}
