using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Aspect;

public abstract class AbstractInterceptor : Attribute, IInterceptor
{
    public abstract Task Invoke(IInvocation invocation, Func<Task> next);
}

public sealed class NoIntercept : Attribute;

public class AttributeInterceptorResolver : IInterceptorResolver
{
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public virtual IReadOnlyList<IInterceptor> ResolveInterceptors(IInvocation invocation)
    {
        Guard.NotNull(invocation);

        if ((invocation.Method ?? invocation.ProxyMethod).IsDefined(typeof(NoIntercept)))
        {
            return Array.Empty<IInterceptor>();
        }

        var baseType = (invocation.Method ?? invocation.ProxyMethod).DeclaringType;
        if (baseType?.IsDefined(typeof(NoIntercept)) == true)
        {
            return Array.Empty<IInterceptor>();
        }

        var list = new List<IInterceptor>();
        var interceptorTypes = new HashSet<Type>();

        // load method interceptor
        if (invocation.Method != null)
        {
            if (invocation.Method.IsDefined(typeof(NoIntercept)))
            {
                return Array.Empty<IInterceptor>();
            }

            foreach (var interceptor in invocation.Method.GetCustomAttributes<AbstractInterceptor>())
            {
                if (interceptorTypes.Add(interceptor.GetType()))
                {
                    list.Add(interceptor);
                }
            }
        }
        else
        {
            foreach (var interceptor in invocation.ProxyMethod.GetCustomAttributes<AbstractInterceptor>())
            {
                if (interceptorTypes.Add(interceptor.GetType()))
                {
                    list.Add(interceptor);
                }
            }
        }

        var parameterTypes = invocation.ProxyMethod.GetParameters().Select(x => x.ParameterType).ToArray();

        while (baseType != null)
        {
            if (baseType.GetMethod(invocation.ProxyMethod.Name, parameterTypes) != null)
            {
                foreach (var interceptor in baseType.GetCustomAttributes<AbstractInterceptor>())
                {
                    if (interceptorTypes.Add(interceptor.GetType()))
                    {
                        list.Add(interceptor);
                    }
                }
                baseType = baseType.BaseType;
            }
            else
            {
                break;
            }
        }

        foreach (var @interface in invocation.ProxyTarget.GetType().GetImplementedInterfaces())
        {
            var interfaceMethod = @interface.GetMethod(invocation.ProxyMethod.Name, parameterTypes);
            if (null != interfaceMethod)
            {
                // interface interceptor
                foreach (var interceptor in @interface.GetCustomAttributes<AbstractInterceptor>())
                {
                    if (interceptorTypes.Add(interceptor.GetType()))
                    {
                        list.Add(interceptor);
                    }
                }
                // interface method interceptor
                foreach (var interceptor in interfaceMethod.GetCustomAttributes<AbstractInterceptor>())
                {
                    if (interceptorTypes.Add(interceptor.GetType()))
                    {
                        list.Add(interceptor);
                    }
                }
            }
        }

        return list;
    }
}
