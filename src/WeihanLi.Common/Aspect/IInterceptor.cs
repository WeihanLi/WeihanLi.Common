using System;
using System.Threading.Tasks;

namespace WeihanLi.Common.Aspect
{
    public interface IInterceptor
    {
        Task Invoke(IInvocation invocation, Func<Task> next);
    }

    internal class TryInvokeInterceptor : IInterceptor
    {
        public async Task Invoke(IInvocation invocation, Func<Task> next)
        {
            try
            {
                await next();
            }
            catch (Exception e)
            {
                throw new AspectInvokeException($"Invoke {invocation.Method.Name} exception", e)
                {
                    ProxyTarget = invocation.ProxyTarget,
                    Target = invocation.Target,
                    Method = invocation.Method,
                };
            }
        }
    }

    internal class DisposableInterceptor : IInterceptor
    {
        public async Task Invoke(IInvocation invocation, Func<Task> next)
        {
            await next();
            // avoid cycling call
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
}
