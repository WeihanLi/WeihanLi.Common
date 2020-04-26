using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Aspect
{
    public class AspectDelegate
    {
        private static readonly ConcurrentDictionary<string, Func<IInvocation, Task>> _aspectDelegates = new ConcurrentDictionary<string, Func<IInvocation, Task>>();

        public static void InvokeWithInterceptors(IInvocation context, IReadOnlyCollection<IInterceptor> interceptors)
        {
            var action = _aspectDelegates.GetOrAdd($"{context.ProxyMethod.DeclaringType}.{context.ProxyMethod}", m =>
            {
                // ReSharper disable once ConvertToLocalFunction
                Func<IInvocation, Task> completeFunc = x =>
                {
                    context.ReturnValue = x.Method?.Invoke(x.Target, x.Parameters);
                    if (context.Method.ReturnType == typeof(void))
                    {
                        return TaskHelper.CompletedTask;
                    }

                    if (context.ReturnValue is Task task)
                    {
                        return task;
                    }
#if NETSTANDARD2_1
                    if (context.ReturnValue is ValueTask valTask)
                    {
                        return valTask.AsTask();
                    }
#endif

                    return TaskHelper.CompletedTask;
                };

                if (interceptors.Count == 0)
                {
                    return completeFunc;
                }

                var builder = PipelineBuilder.CreateAsync(completeFunc);
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

        public static void Invoke(IInvocation context)
        {
            var interceptors =
                DependencyResolver.ResolveService<IInterceptorResolver>()
                ?.ResolveInterceptors(context) ?? ArrayHelper.Empty<IInterceptor>();

            InvokeWithInterceptors(context, interceptors);
        }
    }
}
