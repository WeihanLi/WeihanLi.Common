using System;
using System.Collections.Generic;
using System.Reflection;

namespace WeihanLi.Common.Aspect
{
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
            GenericArguments = methodBase?.GetGenericArguments() ?? Array.Empty<Type>();

            if (proxyMethod.ContainsGenericParameters && GenericArguments.Length > 0)
            {
                ProxyMethod = proxyMethod.MakeGenericMethod(GenericArguments);
            }
            else
            {
                ProxyMethod = proxyMethod;
            }

            Properties = new Dictionary<string, object?>();
        }
    }
}
