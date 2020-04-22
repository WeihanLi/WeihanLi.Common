namespace WeihanLi.Common.Aspect
{
    public interface IMethodInterceptionConfiguration
    {
        IMethodInterceptionConfiguration InterceptWith(IInterceptor interceptor);
    }
}
