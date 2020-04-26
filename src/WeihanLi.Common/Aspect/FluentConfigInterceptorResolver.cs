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
            var interceptors = new List<IInterceptor>(32);
            foreach (var func in FluentAspects.AspectOptions.NoInterceptionConfigurations)
            {
                if (func(invocation))
                {
                    return ArrayHelper.Empty<IInterceptor>();
                }
            }

            foreach (var configuration in FluentAspects.AspectOptions.InterceptionConfigurations)
            {
                if (configuration.Key.Invoke(invocation))
                {
                    foreach (var interceptor in configuration.Value.Interceptors)
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
