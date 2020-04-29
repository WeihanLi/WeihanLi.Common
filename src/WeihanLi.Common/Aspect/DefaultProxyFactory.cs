using JetBrains.Annotations;
using System;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Aspect
{
    public class DefaultProxyFactory : IProxyFactory
    {
        public static readonly IProxyFactory Instance = new DefaultProxyFactory(DefaultProxyTypeFactory.Instance);

        private readonly IProxyTypeFactory _proxyTypeFactory;
        private readonly IServiceProvider _serviceProvider;

        public DefaultProxyFactory(IProxyTypeFactory proxyTypeFactory, IServiceProvider serviceProvider = null)
        {
            _proxyTypeFactory = proxyTypeFactory;
            _serviceProvider = serviceProvider ?? DependencyResolver.Current;
        }

        public object CreateProxy([NotNull] Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            var proxyType = _proxyTypeFactory.CreateProxyType(serviceType);
            var proxy = _serviceProvider.CreateInstance(proxyType);
            if (serviceType.IsClass)
            {
                ProxyUtils.SetProxyTarget(proxy, _serviceProvider.CreateInstance(serviceType));
            }
            return proxy;
        }

        public object CreateProxy([NotNull] Type serviceType, [NotNull] Type implementType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (implementType == null)
            {
                throw new ArgumentNullException(nameof(implementType));
            }

            var proxyType = _proxyTypeFactory.CreateProxyType(serviceType, implementType);
            var proxy = _serviceProvider.CreateInstance(proxyType);
            var target = _serviceProvider.CreateInstance(implementType);
            ProxyUtils.SetProxyTarget(proxy, target);
            return proxy;
        }

        public object CreateProxyWithTarget(Type serviceType, object implement)
        {
            if (null == serviceType)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            if (implement == null)
            {
                throw new ArgumentNullException(nameof(implement));
            }

            var implementType = implement.GetType();
            var proxyType = _proxyTypeFactory.CreateProxyType(serviceType, implementType);
            var proxy = _serviceProvider.CreateInstance(proxyType);
            ProxyUtils.SetProxyTarget(proxy, implement);
            return proxy;
        }

        public object CreateProxy([NotNull] Type serviceType, [NotNull] Type implementType, params object[] parameters)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (implementType == null)
            {
                throw new ArgumentNullException(nameof(implementType));
            }

            var proxyType = _proxyTypeFactory.CreateProxyType(serviceType, implementType);
            var proxy = _serviceProvider.CreateInstance(proxyType);
            var target = _serviceProvider.CreateInstance(implementType, parameters);
            ProxyUtils.SetProxyTarget(proxy, target);
            return proxy;
        }
    }
}
