using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Aspect
{
    public interface IInterceptionConfiguration
    {
        ICollection<IInterceptor> Interceptors { get; }
    }

    internal class InterceptionConfiguration : IInterceptionConfiguration
    {
        public ICollection<IInterceptor> Interceptors { get; }

        public InterceptionConfiguration()
        {
            Interceptors = new List<IInterceptor>();
        }
    }

    public static class InterceptionConfigurationExtensions
    {
        public static IInterceptionConfiguration With(this IInterceptionConfiguration interceptionConfiguration, Func<IInvocation, Func<Task>, Task> interceptFunc)
        {
            if (null != interceptFunc)
            {
                interceptionConfiguration.Interceptors.Add(new DelegateInterceptor(interceptFunc));
            }
            return interceptionConfiguration;
        }

        public static IInterceptionConfiguration With(this IInterceptionConfiguration interceptionConfiguration, IInterceptor interceptor)
        {
            interceptionConfiguration.Interceptors.Add(interceptor);
            return interceptionConfiguration;
        }

        public static IInterceptionConfiguration With<TInterceptor>(this IInterceptionConfiguration interceptionConfiguration) where TInterceptor : IInterceptor, new()
        {
            interceptionConfiguration.With(new TInterceptor());
            return interceptionConfiguration;
        }

        public static IInterceptionConfiguration With<TInterceptor>(this IInterceptionConfiguration interceptionConfiguration, params object[] parameters) where TInterceptor : IInterceptor
        {
            if (null == parameters || parameters.Length == 0)
            {
                interceptionConfiguration.With(NewFuncHelper<TInterceptor>.Instance());
            }
            else
            {
                interceptionConfiguration.With(ActivatorHelper.CreateInstance<TInterceptor>(parameters));
            }
            return interceptionConfiguration;
        }
    }
}
