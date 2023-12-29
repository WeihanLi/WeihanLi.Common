using System.Diagnostics.CodeAnalysis;

namespace WeihanLi.Common.Aspect;

public sealed class DefaultProxyTypeFactory : IProxyTypeFactory
{
    public static readonly IProxyTypeFactory Instance = new DefaultProxyTypeFactory();

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public Type CreateProxyType(Type serviceType)
    {
        Guard.NotNull(serviceType);

        if (serviceType.IsInterface)
        {
            return ProxyUtils.CreateInterfaceProxy(serviceType);
        }
        return ProxyUtils.CreateClassProxy(serviceType, serviceType);
    }

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public Type CreateProxyType(Type serviceType, Type implementType)
    {
        Guard.NotNull(serviceType);

        if (serviceType.IsInterface)
        {
            return ProxyUtils.CreateInterfaceProxy(serviceType, implementType);
        }

        return ProxyUtils.CreateClassProxy(serviceType, implementType);
    }
}
