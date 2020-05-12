using System;
using System.Collections.Generic;

namespace WeihanLi.Common.Aspect
{
    public class FluentAspectOptions
    {
        public readonly Dictionary<Func<IInvocation, bool>, IInterceptionConfiguration> InterceptionConfigurations = new Dictionary<Func<IInvocation, bool>, IInterceptionConfiguration>();

        public HashSet<Func<IInvocation, bool>> NoInterceptionConfigurations { get; } = new HashSet<Func<IInvocation, bool>>();

        public IInterceptorResolver InterceptorResolver { get; set; } = FluentConfigInterceptorResolver.Instance;

        public HashSet<IInvocationEnricher> Enrichers { get; } = new HashSet<IInvocationEnricher>();

        public IProxyFactory ProxyFactory { get; set; } = DefaultProxyFactory.Instance;
    }
}
