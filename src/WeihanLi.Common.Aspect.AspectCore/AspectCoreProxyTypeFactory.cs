namespace WeihanLi.Common.Aspect.AspectCore;

internal sealed class AspectCoreProxyTypeFactory : IProxyTypeFactory
{
    public Type CreateProxyType(Type serviceType)
    {
        Guard.NotNull(serviceType);

        if (serviceType.IsInterface)
        {
            return AspectCoreHelper.ProxyGenerator.TypeGenerator.CreateClassProxyType(serviceType, serviceType);
        }

        return AspectCoreHelper.ProxyGenerator.TypeGenerator.CreateInterfaceProxyType(serviceType);
    }

    public Type CreateProxyType(Type serviceType, Type implementType)
    {
        Guard.NotNull(serviceType);
        Guard.NotNull(implementType);

        if (serviceType.IsInterface)
        {
            return AspectCoreHelper.ProxyGenerator.TypeGenerator.CreateClassProxyType(serviceType, implementType);
        }

        return AspectCoreHelper.ProxyGenerator.TypeGenerator.CreateInterfaceProxyType(serviceType, implementType);
    }
}
