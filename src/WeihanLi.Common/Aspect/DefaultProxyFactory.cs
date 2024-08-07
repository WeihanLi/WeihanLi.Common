﻿using System.Diagnostics.CodeAnalysis;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Aspect;

public sealed class DefaultProxyFactory
    (IProxyTypeFactory proxyTypeFactory, IServiceProvider? serviceProvider = null) : IProxyFactory
{
    public static readonly IProxyFactory Instance = new DefaultProxyFactory(DefaultProxyTypeFactory.Instance);
    private readonly IServiceProvider _serviceProvider = serviceProvider ?? DependencyResolver.Current;

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public object CreateProxy(Type serviceType, object?[] arguments)
    {
        Guard.NotNull(serviceType);

        var proxyType = proxyTypeFactory.CreateProxyType(serviceType);
        var proxy = _serviceProvider.CreateInstance(proxyType, arguments);
        return proxy;
    }

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public object CreateProxy(Type serviceType, Type implementType, params object?[] arguments)
    {
        Guard.NotNull(serviceType);
        Guard.NotNull(implementType);

        var proxyType = proxyTypeFactory.CreateProxyType(serviceType, implementType);
        if (serviceType.IsInterface)
        {
            var implement = _serviceProvider.CreateInstance(implementType, arguments);
            var proxy = _serviceProvider.CreateInstance(proxyType);
            ProxyUtils.SetProxyTarget(proxy, implement);
            return proxy;
        }

        return _serviceProvider.CreateInstance(proxyType, arguments);
    }

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public object CreateProxyWithTarget(Type serviceType, object implement, object?[] arguments)
    {
        Guard.NotNull(serviceType);
        Guard.NotNull(implement);

        var implementType = implement.GetType();

        var proxyType = serviceType.IsClass
                ? proxyTypeFactory.CreateProxyType(serviceType)
                : proxyTypeFactory.CreateProxyType(serviceType, implementType)
            ;
        var proxy = _serviceProvider.CreateInstance(proxyType, arguments);
        ProxyUtils.SetProxyTarget(proxy, implement);

        return proxy;
    }
}
