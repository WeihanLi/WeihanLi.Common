using System;
using System.Collections.Generic;
using System.Reflection;

namespace WeihanLi.Common.Aspect
{
    public class FluentAspects
    {
        internal static readonly Dictionary<Func<MethodInfo, bool>, IReadOnlyCollection<IInterceptor>>
            InterceptorConfigurations = new Dictionary<Func<MethodInfo, bool>, IReadOnlyCollection<IInterceptor>>();

        public static void Configure(Action<FluentAspectOptions> optionsAction)
        {
            var options = new FluentAspectOptions();
            optionsAction?.Invoke(options);
            foreach (var interceptionConfiguration in options._interceptionConfigurations)
            {
                InterceptorConfigurations[interceptionConfiguration.Key] = interceptionConfiguration.Value.Interceptors;
            }
        }
    }

    internal class FluentConfigInterceptorResolver : IInterceptorResolver
    {
        public IReadOnlyCollection<IInterceptor> ResolveInterceptors(IInvocation invocation)
        {
            var interceptors = new List<IInterceptor>(32);
            foreach (var configuration in FluentAspects.InterceptorConfigurations)
            {
                if (configuration.Key.Invoke(invocation.Method ?? invocation.ProxyMethod))
                {
                    foreach (var interceptor in configuration.Value)
                    {
                        if (!interceptors.Exists(x => x.GetType() == interceptor.GetType()))
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
