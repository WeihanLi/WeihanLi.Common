namespace WeihanLi.Common.Aspect;

public sealed class DefaultProxyTypeFactory : IProxyTypeFactory
{
    public static readonly IProxyTypeFactory Instance = new DefaultProxyTypeFactory();

    public Type CreateProxyType(Type serviceType)
    {
        if (null == serviceType)
        {
            throw new ArgumentNullException(nameof(serviceType));
        }

        if (serviceType.IsInterface)
        {
            return ProxyUtils.CreateInterfaceProxy(serviceType);
        }
        return ProxyUtils.CreateClassProxy(serviceType, serviceType);
    }

    public Type CreateProxyType(Type serviceType, Type implementType)
    {
        if (null == serviceType)
        {
            throw new ArgumentNullException(nameof(serviceType));
        }

        if (serviceType.IsInterface)
        {
            return ProxyUtils.CreateInterfaceProxy(serviceType, implementType);
        }

        return ProxyUtils.CreateClassProxy(serviceType, implementType);
    }
}
