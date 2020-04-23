using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Aspect
{
    public class AspectDelegate
    {
        private static readonly ConcurrentDictionary<string, Func<IInvocation, Task>> _aspectDelegates = new ConcurrentDictionary<string, Func<IInvocation, Task>>();

        public static void InvokeAspectDelegate(IInvocation context, IInterceptor[] interceptors)
        {
            var action = _aspectDelegates.GetOrAdd($"{context.ProxyMethod.DeclaringType}.{context.ProxyMethod}", m =>
            {
                var builder = PipelineBuilder.CreateAsync<IInvocation>(x =>
                {
                    var returnVal = x.Method?.Invoke(x.Target, x.Parameters);

                    if (returnVal is Task task)
                    {
                        return task;
                    }
#if NETSTANDARD2_1
                    if (returnVal is ValueTask valTask)
                    {
                        return valTask.AsTask();
                    }
#endif

                    return Task.FromResult(returnVal);
                });
                foreach (var interceptor in interceptors)
                {
                    builder.Use(interceptor.Invoke);
                }
                return builder.Build();
            });
            action.Invoke(context);

            // check for return value
            if (context.ProxyMethod.ReturnType != typeof(void))
            {
                if (context.ReturnValue == null && context.ProxyMethod.ReturnType.IsValueType)
                {
                    context.ReturnValue = Activator.CreateInstance(context.ProxyMethod.ReturnType);
                }
            }
        }

        public static void InvokeAspectDelegate(IInvocation context)
        {
            var interceptors = DependencyResolver.ResolveService<IInterceptorResolver>()
                ?.ResolveInterceptors(context.ProxyMethod) ?? ArrayHelper.Empty<IInterceptor>();

            InvokeAspectDelegate(context, interceptors);
        }
    }
}
