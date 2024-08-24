﻿using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Aspect;

public static class AspectDelegate
{
    [RequiresUnreferencedCode("Unreferenced code may be used.")]
    public static void Invoke(IInvocation context)
    {
        InvokeInternal(context, null, null);
    }

    [RequiresUnreferencedCode("Unreferenced code may be used.")]
    public static void InvokeWithInterceptors(IInvocation invocation, IReadOnlyList<IInterceptor>? interceptors)
    {
        InvokeInternal(invocation, interceptors, null);
    }

    [RequiresUnreferencedCode("Unreferenced code may be used.")]
    public static void InvokeWithCompleteFunc(IInvocation invocation, Func<IInvocation, Task>? completeFunc)
    {
        InvokeInternal(invocation, null, completeFunc);
    }

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static void InvokeInternal(IInvocation invocation, IReadOnlyList<IInterceptor>? interceptors, Func<IInvocation, Task>? completeFunc)
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
        if (invocation.ProxyMethod.ReturnType != typeof(void) && invocation.ReturnValue == null)
        {
            if (invocation.ProxyMethod.ReturnType.IsValueType)
            {
                invocation.ReturnValue = invocation.ProxyMethod.ReturnType.GetDefaultValue();
            }

            if (invocation.ProxyMethod.ReturnType == typeof(Task))
            {
                invocation.ReturnValue = Task.CompletedTask;
            }

            if (invocation.ProxyMethod.ReturnType.IsGenericType
                && invocation.ProxyMethod.ReturnType.IsAssignableTo<Task>())
            {
                var resultType = invocation.ProxyMethod.ReturnType.GetGenericArguments()[0];
                invocation.ReturnValue = Task.FromResult(resultType.GetDefaultValue());
            }

            if (invocation.ProxyMethod.ReturnType == typeof(ValueTask))
            {
                invocation.ReturnValue = default(ValueTask);
            }
            if (invocation.ProxyMethod.ReturnType.IsGenericType
                && invocation.ProxyMethod.ReturnType.IsAssignableTo<ValueTask>())
            {
                var resultType = invocation.ProxyMethod.ReturnType.GetGenericArguments()[0];
                invocation.ReturnValue = new ValueTask(Task.FromResult(resultType.GetDefaultValue()));
            }
        }
    }

    [RequiresDynamicCode("Defining a dynamic assembly requires dynamic code.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    private static Func<IInvocation, Task> GetAspectDelegate(IInvocation invocation, IReadOnlyList<IInterceptor>? interceptors, Func<IInvocation, Task>? completeFunc)
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
                        var @delegate = (Delegate)Guard.NotNull(Activator.CreateInstance(delegateType, x.Target, ptr));
                        invocation.ReturnValue = @delegate.DynamicInvoke(x.Arguments);
                    }
                    else
                    {
                        invocation.ReturnValue = x.Method.Invoke(x.Target, x.Arguments);
                    }
                }

                if (invocation.ProxyMethod.ReturnType == typeof(void))
                {
                    return Task.CompletedTask;
                }
                if (invocation.ReturnValue is Task task)
                {
                    return task;
                }
                if (invocation.ReturnValue is ValueTask valTask)
                {
                    return valTask.AsTask();
                }

                return Task.CompletedTask;
            };
        }

        // ReSharper disable once ConvertIfStatementToNullCoalescingAssignment
        if (null == interceptors)
        {
            interceptors = FluentAspects.AspectOptions.InterceptorResolver
                .ResolveInterceptors(invocation);
        }

        if (interceptors.Count <= 1 && interceptors[0] is TryInvokeInterceptor)
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
