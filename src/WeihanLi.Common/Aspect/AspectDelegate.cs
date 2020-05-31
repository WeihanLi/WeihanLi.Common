using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Aspect
{
    public static class AspectDelegate
    {
        public static void Invoke(IInvocation context)
        {
            InvokeInternal(context, null, null);
        }

        public static void InvokeWithInterceptors(IInvocation invocation, IReadOnlyCollection<IInterceptor> interceptors)
        {
            InvokeInternal(invocation, interceptors, null);
        }

        public static void InvokeWithCompleteFunc(IInvocation invocation, Func<IInvocation, Task> completeFunc)
        {
            InvokeInternal(invocation, null, completeFunc);
        }

        public static void InvokeInternal(IInvocation invocation, IReadOnlyCollection<IInterceptor> interceptors, Func<IInvocation, Task> completeFunc)
        {
            // enrich
            foreach (var enricher in FluentAspects.AspectOptions.Enrichers)
            {
                try
                {
                    enricher.Enrich(invocation);
                }
                catch (Exception ex)
                {
                    InvokeHelper.OnInvokeException?.Invoke(ex);
                }
            }

            // invoke delegate
            var action = GetAspectDelegate(invocation, interceptors, completeFunc);
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

            // ensure return value
            if (invocation.ProxyMethod.ReturnType != typeof(void))
            {
                if (invocation.ReturnValue == null && invocation.ProxyMethod.ReturnType.IsValueType)
                {
                    invocation.ReturnValue = invocation.ProxyMethod.ReturnType.GetDefaultValue();
                }
            }
        }

        private static Func<IInvocation, Task> GetAspectDelegate(IInvocation invocation, IReadOnlyCollection<IInterceptor> interceptors, Func<IInvocation, Task> completeFunc)
        {
            // ReSharper disable once ConvertToLocalFunction
            // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
            if (null == completeFunc)
            {
                completeFunc = x =>
                {
                    if (x.Method != null && !x.Method.IsAbstract)
                    {
                        if (x.Target == x.ProxyTarget)
                        {
                            // https://stackoverflow.com/questions/2323401/how-to-call-base-base-method
                            var ptr = x.Method.MethodHandle.GetFunctionPointer();
                            var delegateType = DelegateHelper.GetDelegateType(x.Method);
                            var @delegate = (Delegate)Activator.CreateInstance(delegateType, x.Target, ptr);
                            invocation.ReturnValue = @delegate.DynamicInvoke(x.Arguments);
                        }
                        else
                        {
                            invocation.ReturnValue = x.Method.Invoke(x.Target, x.Arguments);
                        }
                    }

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
                    if (null == invocation.ReturnValue)
                    {
                        if (invocation.ProxyMethod.ReturnType.IsGenericType
                        && invocation.ProxyMethod.ReturnType.IsAssignableTo<Task>())
                        {
                            var resultType = invocation.ProxyMethod.ReturnType.GetGenericArguments()[0];
                            return Task.FromResult(resultType.GetDefaultValue());
                        }

#if NETSTANDARD2_1

                        if (invocation.ProxyMethod.ReturnType.IsGenericType
                                                && invocation.ProxyMethod.ReturnType.IsAssignableTo<ValueTask>())
                        {
                            var resultType = invocation.ProxyMethod.ReturnType.GetGenericArguments()[0];
                            return Task.FromResult(resultType.GetDefaultValue());
                        }
#endif
                    }

                    return TaskHelper.CompletedTask;
                };
            }

            // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
            if (null == interceptors)
            {
                interceptors = (FluentAspects.AspectOptions.InterceptorResolver ??
                                FluentConfigInterceptorResolver.Instance)
                    .ResolveInterceptors(invocation) ?? ArrayHelper.Empty<IInterceptor>();
            }

            if (interceptors.Count <= 1)
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
    }
}
