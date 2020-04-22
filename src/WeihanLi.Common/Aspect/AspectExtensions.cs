using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Aspect
{
    public static class AspectExtensions
    {
        public static IMethodInterceptionConfiguration InterceptWith<TInterceptor>(
            this IMethodInterceptionConfiguration interceptionConfiguration) where TInterceptor : IInterceptor, new()
        {
            return interceptionConfiguration.InterceptWith(new TInterceptor());
        }

        public static IMethodInterceptionConfiguration InterceptWith<TInterceptor>(
            this IMethodInterceptionConfiguration interceptionConfiguration, params object[] parameters) where TInterceptor : IInterceptor, new()
        {
            return interceptionConfiguration.InterceptWith(ActivatorHelper.CreateInstance<TInterceptor>(parameters));
        }
    }
}
