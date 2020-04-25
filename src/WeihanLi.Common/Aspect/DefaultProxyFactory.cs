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
            return (TService)_serviceProvider.GetServiceOrCreateInstance(type);
        }

        public TService CreateProxy<TService, TImplement>() where TImplement : TService
            where TService : class
        {
            var type = _proxyTypeFactory.CreateProxyType(typeof(TService), typeof(TImplement));
            return (TService)_serviceProvider.GetServiceOrCreateInstance(type);
        }

        public TService CreateProxy<TService, TImplement>(object[] parameters) where TImplement : TService
            where TService : class
        {
            var type = _proxyTypeFactory.CreateProxyType(typeof(TService), typeof(TImplement));
            return (TService)_serviceProvider.GetServiceOrCreateInstance(type);
        }
    }
}
