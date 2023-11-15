namespace WeihanLi.Common.Aspect.Castle;

internal sealed class CastleProxyTypeFactory : IProxyTypeFactory
{
    public Type CreateProxyType(Type serviceType)
    {
        Guard.NotNull(serviceType);

        if (serviceType.IsInterface)
        {
            return CastleHelper.ProxyBuilder.CreateInterfaceProxyTypeWithTargetInterface(serviceType, Type.EmptyTypes,
                CastleHelper.ProxyGenerationOptions);
        }

        return CastleHelper.ProxyBuilder.CreateClassProxyType(serviceType, Type.EmptyTypes,
            CastleHelper.ProxyGenerationOptions);
    }

    public Type CreateProxyType(Type serviceType, Type implementType)
    {
        Guard.NotNull(serviceType);
        Guard.NotNull(implementType);

        if (serviceType.IsInterface)
        {
            return CastleHelper.ProxyBuilder.CreateInterfaceProxyTypeWithTarget(serviceType, Type.EmptyTypes, implementType,
                CastleHelper.ProxyGenerationOptions);
        }

        return CastleHelper.ProxyBuilder.CreateClassProxyTypeWithTarget(implementType, Type.EmptyTypes,
            CastleHelper.ProxyGenerationOptions);
    }
}
