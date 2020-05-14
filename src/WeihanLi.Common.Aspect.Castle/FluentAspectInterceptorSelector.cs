using Castle.DynamicProxy;
using System;
using System.Reflection;

namespace WeihanLi.Common.Aspect.Castle
{
    internal sealed class FluentAspectInterceptorSelector : IInterceptorSelector
    {
        public global::Castle.DynamicProxy.IInterceptor[] SelectInterceptors(Type type, MethodInfo method, global::Castle.DynamicProxy.IInterceptor[] interceptors)
        {
            return new global::Castle.DynamicProxy.IInterceptor[] { new FluentAspectInterceptor() };
        }
    }
}
