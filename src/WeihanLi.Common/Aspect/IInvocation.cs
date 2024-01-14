using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace WeihanLi.Common.Aspect;

public interface IInvocation
{
    MethodInfo ProxyMethod { get; }

    object ProxyTarget { get; }

    MethodInfo? Method { get; }

    object? Target { get; }

    object[] Arguments { get; }

    Type[] GenericArguments { get; }

    object? ReturnValue { get; set; }

    Dictionary<string, object?> Properties { get; }
}

public class AspectInvocation : IInvocation
{
    public MethodInfo ProxyMethod { get; }

    public object ProxyTarget { get; }

    public MethodInfo? Method { get; }

    public object? Target { get; }

    public object[] Arguments { get; }

    public Type[] GenericArguments { get; }

    public object? ReturnValue { get; set; }

    public Dictionary<string, object?> Properties { get; }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public AspectInvocation(
        MethodInfo proxyMethod,
        MethodInfo? methodBase,
        object proxyTarget,
        object? target,
        object[] arguments)
    {
        Method = methodBase;
        ProxyTarget = proxyTarget;
        Target = target;
        Arguments = arguments;
        GenericArguments = methodBase?.GetGenericArguments() ?? [];

        if (proxyMethod.ContainsGenericParameters && GenericArguments.Length > 0)
        {
            ProxyMethod = proxyMethod.MakeGenericMethod(GenericArguments);
        }
        else
        {
            ProxyMethod = proxyMethod;
        }

        Properties = [];
    }
}
