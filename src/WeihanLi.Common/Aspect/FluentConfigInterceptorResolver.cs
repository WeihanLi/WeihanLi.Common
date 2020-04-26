using System.Collections.Generic;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Aspect
{
    internal class FluentConfigInterceptorResolver : IInterceptorResolver
    {
        public IReadOnlyCollection<IInterceptor> ResolveInterceptors(IInvocation invocation)
        {
            var interceptors = new List<IInterceptor>(32);
            foreach (var func in FluentAspects.AspectOptions._noInterceptionConfigurations)
            {
                if (func(invocation))
                {
                    return ArrayHelper.Empty<IInterceptor>();
                }
            }

            foreach (var configuration in FluentAspects.AspectOptions._interceptionConfigurations)
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
