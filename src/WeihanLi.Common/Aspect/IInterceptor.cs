﻿namespace WeihanLi.Common.Aspect;

public interface IInterceptor
{
    Task Invoke(IInvocation invocation, Func<Task> next);
}

public class TryInvokeInterceptor : IInterceptor
{
    public async Task Invoke(IInvocation invocation, Func<Task> next)
    {
        try
        {
            await next();
        }
        catch (Exception e)
        {
            throw new AspectInvokeException(invocation, e);
        }
    }
}

internal sealed class DisposableInterceptor : IInterceptor
{
    public async Task Invoke(IInvocation invocation, Func<Task> next)
    {
        await next();
        // avoid cycling call
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (invocation.Target != null
            && invocation.Target != invocation.ProxyTarget
            && invocation.ProxyMethod.Name == nameof(IDisposable.Dispose)
            && invocation.Target is IDisposable disposable
        )
        {
            disposable.Dispose();
        }
    }
}
