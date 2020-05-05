using System.Linq;

namespace WeihanLi.Common.Aspect.Castle
{
    internal class FluentAspectInterceptor : global::Castle.DynamicProxy.IInterceptor
    {
        public void Intercept(global::Castle.DynamicProxy.IInvocation invocation)
        {
            var aspectInvocation = new AspectInvocation(
                invocation.GetConcreteMethod(),
                invocation.GetConcreteMethodInvocationTarget(),
                invocation.Proxy,
                invocation.InvocationTarget,
                invocation.Arguments
                );

            if (
                FluentAspects.AspectOptions.NoInterceptionConfigurations.Any(x => x.Invoke(aspectInvocation)))
            {
                return;
            }
            AspectDelegate.Invoke(aspectInvocation);
        }
    }
}
