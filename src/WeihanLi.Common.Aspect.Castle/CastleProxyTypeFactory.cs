using Castle.DynamicProxy;
using System;

namespace WeihanLi.Common.Aspect.Castle
{
    internal class CastleProxyTypeFactory : IProxyTypeFactory
    {
        public Type CreateProxyType(Type serviceType)
        {
            if (serviceType.IsInterface)
            {
                return CastleHelper.ProxyGenerator.ProxyBuilder
                    .CreateInterfaceProxyTypeWithoutTarget(serviceType, Type.EmptyTypes, ProxyGenerationOptions.Default);
            }
            return CastleHelper.ProxyGenerator.ProxyBuilder
                .CreateClassProxyType(serviceType, Type.EmptyTypes, ProxyGenerationOptions.Default);
        }

        public Type CreateProxyType(Type serviceType, Type implementType)
        {
            if (serviceType.IsInterface)
            {
                return CastleHelper.ProxyGenerator.ProxyBuilder
                    .CreateInterfaceProxyTypeWithTarget(serviceType, Type.EmptyTypes, implementType, ProxyGenerationOptions.Default);
            }
            return CastleHelper.ProxyGenerator.ProxyBuilder
                .CreateClassProxyTypeWithTarget(implementType, Type.EmptyTypes, ProxyGenerationOptions.Default);
        }
    }
}
