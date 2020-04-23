using System.Reflection;

namespace WeihanLi.Common.Aspect
{
    public interface IInterceptorResolver
    {
        IInterceptor[] ResolveInterceptors(MethodInfo methodInfo);
    }
}
