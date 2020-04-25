using System.Collections.Generic;

namespace WeihanLi.Common.Aspect
{
    public interface IInterceptorResolver
    {
        IReadOnlyCollection<IInterceptor> ResolveInterceptors(IInvocation invocation);
    }
}
