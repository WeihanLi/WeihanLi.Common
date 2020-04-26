using System;
using System.Collections.Generic;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Aspect
{
    public class FluentConfigInterceptorResolver : IInterceptorResolver
    {
        public static readonly IInterceptorResolver Instance = new FluentConfigInterceptorResolver();

        private FluentConfigInterceptorResolver()
        {
        }

        public IReadOnlyCollection<IInterceptor> ResolveInterceptors(IInvocation invocation)
        {
            foreach (var func in FluentAspects.AspectOptions.NoInterceptionConfigurations)
            {
                if (func(invocation))
                {
                    return ArrayHelper.Empty<IInterceptor>();
                }
            }
            var interceptorTypes = new HashSet<Type>();
            var interceptors = new List<IInterceptor>();
            foreach (var configuration in FluentAspects.AspectOptions.InterceptionConfigurations)
            {
                if (configuration.Key.Invoke(invocation))
                {
                    foreach (var interceptor in configuration.Value.Interceptors)
                    {
                        if (interceptorTypes.Add(interceptor.GetType()))
                        {
                            interceptors.Add(interceptor);
                        }
                    }
                }
            }
            return interceptors;
        }
    }
}
