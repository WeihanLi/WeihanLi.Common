using System;
using System.Collections.Generic;

namespace WeihanLi.Common.Aspect
{
    public class FluentAspectOptions
    {
        public Dictionary<Func<IInvocation, bool>, IInterceptionConfiguration> InterceptionConfigurations = new Dictionary<Func<IInvocation, bool>, IInterceptionConfiguration>();

        public HashSet<Func<IInvocation, bool>> NoInterceptionConfigurations { get; } = new HashSet<Func<IInvocation, bool>>();

        public IInterceptorResolver InterceptorResolver { get; set; } = FluentConfigInterceptorResolver.Instance;

        public IProxyFactory ProxyFactory { get; set; } = DefaultProxyFactory.Instance;

        public bool NoIntercept(Func<IInvocation, bool> predict)
        {
            return NoInterceptionConfigurations.Add(predict);
        }

        public IInterceptionConfiguration Intercept(Func<IInvocation, bool> predict)
        {
            if (null == predict)
            {
                throw new ArgumentNullException(nameof(predict));
            }
            if (InterceptionConfigurations.TryGetValue
                (predict, out var interceptionConfiguration))
            {
                return interceptionConfiguration;
            }
            interceptionConfiguration = new InterceptionConfiguration();
            InterceptionConfigurations[predict] = interceptionConfiguration;
            return interceptionConfiguration;
        }
    }
}
