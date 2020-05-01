using System;

namespace WeihanLi.Common.Aspect
{
    public interface IProxyFactory
    {
        object CreateProxy(Type serviceType);

        object CreateProxy(Type serviceType, Type implementType);

        object CreateProxyWithTarget(Type serviceType, object implement);
    }
}
