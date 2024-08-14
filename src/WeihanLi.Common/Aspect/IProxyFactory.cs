using System.Diagnostics.CodeAnalysis;

namespace WeihanLi.Common.Aspect;

public interface IProxyFactory
{
    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    object CreateProxy(Type serviceType, object?[] arguments);

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    object CreateProxy(Type serviceType, Type implementType, params object?[] arguments);

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    object CreateProxyWithTarget(Type serviceType, object implement, object?[] arguments);
}
