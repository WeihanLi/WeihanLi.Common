using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Aspect.AspectCore;

internal sealed class AspectCoreProxyFactory : IProxyFactory
{
    public static readonly IProxyFactory Instance = new AspectCoreProxyFactory(DependencyResolver.Current);

    private readonly IServiceProvider _serviceProvider;

    public AspectCoreProxyFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public object CreateProxy(Type serviceType, object?[] arguments)
    {
        if (null == serviceType)
            throw new ArgumentNullException(nameof(serviceType));

        if (serviceType.IsInterface)
            return AspectCoreHelper.ProxyGenerator.CreateInterfaceProxy(serviceType);

        var ctorArguments = ActivatorHelper.GetBestConstructorArguments(_serviceProvider, serviceType, arguments);
        return AspectCoreHelper.ProxyGenerator.CreateClassProxy(serviceType, serviceType, ctorArguments);
    }

    public object CreateProxy(Type serviceType, Type implementType, params object?[] arguments)
    {
        if (null == serviceType)
            throw new ArgumentNullException(nameof(serviceType));
        if (null == implementType)
            throw new ArgumentNullException(nameof(implementType));

        if (serviceType.IsInterface)
        {
            var implementInstance = _serviceProvider.CreateInstance(implementType);
            return AspectCoreHelper.ProxyGenerator.CreateInterfaceProxy(serviceType, implementInstance);
        }
        var ctorArguments = ActivatorHelper.GetBestConstructorArguments(_serviceProvider, implementType, arguments);
        return AspectCoreHelper.ProxyGenerator.CreateClassProxy(serviceType, implementType, ctorArguments);
    }

    public object CreateProxyWithTarget(Type serviceType, object implement, object?[] arguments)
    {
        if (null == serviceType)
            throw new ArgumentNullException(nameof(serviceType));
        if (null == implement)
            throw new ArgumentNullException(nameof(implement));

        if (serviceType.IsInterface)
            return AspectCoreHelper.ProxyGenerator.CreateInterfaceProxy(serviceType, implement);

        return AspectCoreHelper.ProxyGenerator.CreateClassProxy(serviceType, implement.GetType(), arguments);
    }
}
