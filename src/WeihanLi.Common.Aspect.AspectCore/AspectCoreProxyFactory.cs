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
        Guard.NotNull(serviceType);

        if (serviceType.IsInterface)
            return AspectCoreHelper.ProxyGenerator.CreateInterfaceProxy(serviceType);

        var ctorArguments = ActivatorHelper.GetBestConstructorArguments(_serviceProvider, serviceType, arguments);
        return AspectCoreHelper.ProxyGenerator.CreateClassProxy(serviceType, serviceType, ctorArguments);
    }

    public object CreateProxy(Type serviceType, Type implementType, params object?[] arguments)
    {
        Guard.NotNull(serviceType);
        Guard.NotNull(implementType);

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
        Guard.NotNull(serviceType);
        Guard.NotNull(implement);

        if (serviceType.IsInterface)
            return AspectCoreHelper.ProxyGenerator.CreateInterfaceProxy(serviceType, implement);

        return AspectCoreHelper.ProxyGenerator.CreateClassProxy(serviceType, implement.GetType(), arguments);
    }
}
