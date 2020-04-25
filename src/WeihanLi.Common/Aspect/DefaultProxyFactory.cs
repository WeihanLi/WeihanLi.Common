using System;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Aspect
{
    public class DefaultProxyFactory : IProxyFactory
    {
        private readonly IProxyTypeFactory _proxyTypeFactory;
        private readonly IServiceProvider _serviceProvider;

        public DefaultProxyFactory(IProxyTypeFactory proxyTypeFactory, IServiceProvider serviceProvider = null)
        {
            _proxyTypeFactory = proxyTypeFactory;
            _serviceProvider = serviceProvider ?? DependencyResolver.Current;
        }

        public TService CreateProxy<TService>() where TService : class
        {
            var type = _proxyTypeFactory.CreateProxyType(typeof(TService));
            var proxy = (TService)_serviceProvider.CreateInstance(type);
            if (type.IsClass)
            {
                ProxyUtils.SetProxyTarget(proxy, _serviceProvider.CreateInstance(typeof(TService)));
            }
            return proxy;
        }

        public TService CreateProxy<TService, TImplement>() where TImplement : TService
            where TService : class
        {
            var type = _proxyTypeFactory.CreateProxyType(typeof(TService), typeof(TImplement));
            var proxy = (TService)_serviceProvider.CreateInstance(type);
            ProxyUtils.SetProxyTarget(proxy, _serviceProvider.CreateInstance(typeof(TImplement)));
            return proxy;
        }

        public TService CreateProxy<TService, TImplement>(object[] parameters) where TImplement : TService
            where TService : class
        {
            var type = _proxyTypeFactory.CreateProxyType(typeof(TService), typeof(TImplement));
            var proxy = (TService)_serviceProvider.CreateInstance(type, parameters);
            ProxyUtils.SetProxyTarget(proxy, _serviceProvider.CreateInstance(typeof(TImplement), parameters));
            return proxy;
        }
    }
}
