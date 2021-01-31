using System;

namespace WeihanLi.Common.Aspect
{
    public interface IProxyFactory
    {
        object CreateProxy(Type serviceType, object?[] arguments);

        object CreateProxy(Type serviceType, Type implementType, params object?[] arguments);

        object CreateProxyWithTarget(Type serviceType, object implement, object?[] arguments);
    }
}
