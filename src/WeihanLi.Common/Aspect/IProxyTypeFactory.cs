using System;

namespace WeihanLi.Common.Aspect;

public interface IProxyTypeFactory
{
    Type CreateProxyType(Type serviceType);

    Type CreateProxyType(Type serviceType, Type implementType);
}
