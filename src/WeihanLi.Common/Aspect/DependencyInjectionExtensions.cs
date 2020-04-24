using Microsoft.Extensions.DependencyInjection;
using System;

namespace WeihanLi.Common.Aspect
{
    public static class DependencyInjectionExtensions
    {
        public static IAspectBuilder AddFluentAspect(this IServiceCollection serviceCollection)
        {
            if (null == serviceCollection)
                throw new ArgumentNullException(nameof(serviceCollection));

            serviceCollection.AddSingleton<IProxyTypeFactory, DefaultProxyTypeFactory>();
            serviceCollection.AddSingleton<IProxyFactory, DefaultProxyFactory>();
            serviceCollection.AddSingleton<IInterceptorResolver, FluentConfigInterceptorResolver>();

            return new AspectBuilder(serviceCollection);
        }
    }
}
