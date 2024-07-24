namespace WeihanLi.Common.Aspect;

public interface IProxyTypeFactory
{
    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    Type CreateProxyType(Type serviceType);

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    Type CreateProxyType(Type serviceType, Type implementType);
}
