using AspectCore.DynamicProxy;

namespace WeihanLi.Common.Aspect.AspectCore;

public sealed class AspectCoreFluentAspectInterceptor : global::AspectCore.DynamicProxy.IInterceptor
{
    public async Task Invoke(AspectContext context, global::AspectCore.DynamicProxy.AspectDelegate next)
    {
        var aspectInvocation = new AspectInvocation(
            context.ProxyMethod,
            context.ImplementationMethod,
            context.Proxy,
            context.Implementation,
            context.Parameters
            );

        if (FluentAspects.AspectOptions.NoInterceptionConfigurations.Any(x => x.Invoke(aspectInvocation)))
        {
            await next(context);
        }
        else
        {
            // ReSharper disable once ConvertToLocalFunction
            Func<IInvocation, Task> completeFunc = async c =>
            {
                await next(context);
                aspectInvocation.ReturnValue = context.ReturnValue;
            };
            AspectDelegate.InvokeWithCompleteFunc(aspectInvocation, completeFunc);
        }
    }

    public bool AllowMultiple => false;
    public bool Inherited { get; set; } = false;
    public int Order { get; set; }
}
