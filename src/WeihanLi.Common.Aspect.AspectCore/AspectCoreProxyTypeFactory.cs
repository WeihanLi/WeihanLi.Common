using System;

namespace WeihanLi.Common.Aspect.AspectCore
{
    internal sealed class AspectCoreProxyTypeFactory : IProxyTypeFactory
    {
        public Type CreateProxyType(Type serviceType)
        {
            if (null == serviceType)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            if (serviceType.IsInterface)
            {
                return AspectCoreHelper.ProxyGenerator.TypeGenerator.CreateClassProxyType(serviceType, serviceType);
            }

            return AspectCoreHelper.ProxyGenerator.TypeGenerator.CreateInterfaceProxyType(serviceType);
        }

        public Type CreateProxyType(Type serviceType, Type implementType)
        {
            if (null == serviceType)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            if (null == implementType)
            {
                throw new ArgumentNullException(nameof(implementType));
            }
            if (serviceType.IsInterface)
            {
                return AspectCoreHelper.ProxyGenerator.TypeGenerator.CreateClassProxyType(serviceType, implementType);
            }

            return AspectCoreHelper.ProxyGenerator.TypeGenerator.CreateInterfaceProxyType(serviceType, implementType);
        }
    }
}
