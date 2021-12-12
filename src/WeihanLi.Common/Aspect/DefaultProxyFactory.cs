using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Aspect;

public sealed class DefaultProxyFactory : IProxyFactory
{
    public static readonly IProxyFactory Instance = new DefaultProxyFactory(DefaultProxyTypeFactory.Instance);

    private readonly IProxyTypeFactory _proxyTypeFactory;
    private readonly IServiceProvider _serviceProvider;

    public DefaultProxyFactory(IProxyTypeFactory proxyTypeFactory, IServiceProvider? serviceProvider = null)
    {
        _proxyTypeFactory = proxyTypeFactory;
        _serviceProvider = serviceProvider ?? DependencyResolver.Current;
    }

    public object CreateProxy(Type serviceType, object?[] arguments)
    {
        if (serviceType == null)
        {
            throw new ArgumentNullException(nameof(serviceType));
        }

        var proxyType = _proxyTypeFactory.CreateProxyType(serviceType);
        var proxy = _serviceProvider.CreateInstance(proxyType, arguments);
        return proxy;
    }

    public object CreateProxy(Type serviceType, Type implementType, params object?[] arguments)
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
        if (serviceType.IsInterface)
        {
            var implement = _serviceProvider.CreateInstance(implementType, arguments);
            var proxy = _serviceProvider.CreateInstance(proxyType);
            ProxyUtils.SetProxyTarget(proxy, implement);
            return proxy;
        }

        return _serviceProvider.CreateInstance(proxyType, arguments);
    }

    public object CreateProxyWithTarget(Type serviceType, object implement, object?[] arguments)
    {
        if (null == serviceType)
        {
            throw new ArgumentNullException(nameof(serviceType));
        }
        if (null == implement)
        {
            throw new ArgumentNullException(nameof(implement));
        }

        var implementType = implement.GetType();

        var proxyType = serviceType.IsClass
                ? _proxyTypeFactory.CreateProxyType(serviceType)
                : _proxyTypeFactory.CreateProxyType(serviceType, implementType)
            ;
        var proxy = _serviceProvider.CreateInstance(proxyType, arguments);
        ProxyUtils.SetProxyTarget(proxy, implement);

        return proxy;
    }
}
