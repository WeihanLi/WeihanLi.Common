using System.Collections.Generic;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Aspect
{
    public interface IInterceptionConfiguration
    {
        IInterceptionConfiguration With(params IInterceptor[] interceptors);
    }

    internal class InterceptionConfiguration : IInterceptionConfiguration
    {
        public List<IInterceptor> Interceptors { get; }

        public InterceptionConfiguration(List<IInterceptor> interceptors)
        {
            Interceptors = interceptors;
        }

        public IInterceptionConfiguration With(params IInterceptor[] interceptors)
        {
            if (interceptors != null)
            {
                foreach (var interceptor in interceptors)
                {
                    Interceptors.Add(interceptor);
                }
            }

            return this;
        }
    }

    public static class InterceptionConfigurationExtensions
    {
        public static IInterceptionConfiguration With<TInterceptor>(this IInterceptionConfiguration interceptionConfiguration) where TInterceptor : IInterceptor, new()
        {
            interceptionConfiguration.With(new TInterceptor());
            return interceptionConfiguration;
        }

        public static IInterceptionConfiguration With<TInterceptor>(this IInterceptionConfiguration interceptionConfiguration, params object[] parameters) where TInterceptor : IInterceptor
        {
            interceptionConfiguration.With(ActivatorHelper.CreateInstance<TInterceptor>(parameters));
            return interceptionConfiguration;
        }
    }
}
