using System;
using System.Linq;
using System.Threading.Tasks;

namespace WeihanLi.Common.Aspect.Castle
{
    internal class FluentAspectInterceptor : global::Castle.DynamicProxy.IInterceptor
    {
        public void Intercept(global::Castle.DynamicProxy.IInvocation invocation)
        {
            var aspectInvocation = new AspectInvocation(
                invocation.GetConcreteMethod(),
                null,
                invocation.Proxy,
                invocation.InvocationTarget,
                invocation.Arguments
            );

            var hasTarget = null != invocation.InvocationTarget
                            && null != invocation.MethodInvocationTarget
                            && null != invocation.TargetType;

            if (FluentAspects.AspectOptions.NoInterceptionConfigurations.
                Any(x => x.Invoke(aspectInvocation)))
            {
                if (hasTarget)
                {
                    invocation.Proceed();
                }
                return;
            }

            Func<IInvocation, Task> completeFunc = null;

            if (hasTarget)
            {
                completeFunc = c =>
                {
                    invocation.Proceed();
                    c.ReturnValue = invocation.ReturnValue;
                    return Task.CompletedTask;
                };
            }

            AspectDelegate.InvokeWithCompleteFunc(aspectInvocation, completeFunc);
        }
    }
}
