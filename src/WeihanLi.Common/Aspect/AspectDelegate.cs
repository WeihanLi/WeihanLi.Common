using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Aspect
{
    public class AspectDelegate
    {
        public static void InvokeWithInterceptors(IInvocation invocation, IReadOnlyCollection<IInterceptor> interceptors)
        {
            var action = GetAspectDelegate(invocation, interceptors);
            var task = action.Invoke(invocation);
            if (!task.IsCompleted)
            {
                // await task to be completed
                task.ConfigureAwait(false).GetAwaiter().GetResult();
            }

            if (task.Exception != null)
            {
                var exception = task.Exception.Unwrap();
                throw exception;
            }

            // check for return value
            if (invocation.ProxyMethod.ReturnType != typeof(void))
            {
                if (invocation.ReturnValue == null && invocation.ProxyMethod.ReturnType.IsValueType)
                {
                    invocation.ReturnValue = Activator.CreateInstance(invocation.ProxyMethod.ReturnType);
                }
            }
        }

        private static Func<IInvocation, Task> GetAspectDelegate(IInvocation invocation, IReadOnlyCollection<IInterceptor> interceptors)
        {
            // ReSharper disable once ConvertToLocalFunction
            Func<IInvocation, Task> completeFunc = x =>
            {
                invocation.ReturnValue = x.Method?.Invoke(x.Target, x.Arguments);
                if (invocation.ProxyMethod.ReturnType == typeof(void))
                {
                    return TaskHelper.CompletedTask;
                }

                if (invocation.ReturnValue is Task task)
                {
                    return task;
                }
#if NETSTANDARD2_1
                if (invocation.ReturnValue is ValueTask valTask)
                {
                    return valTask.AsTask();
                }
#endif

                return TaskHelper.CompletedTask;
            };

            // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
            if (null == interceptors)
            {
                interceptors = (FluentAspects.AspectOptions.InterceptorResolver ??
                                FluentConfigInterceptorResolver.Instance)
                    .ResolveInterceptors(invocation) ?? ArrayHelper.Empty<IInterceptor>();
            }

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
        }

        public static void Invoke(IInvocation context)
        {
            InvokeWithInterceptors(context, null);
        }
    }
}
