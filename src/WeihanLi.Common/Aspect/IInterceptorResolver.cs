using System.Collections.Generic;

namespace WeihanLi.Common.Aspect;

public interface IInterceptorResolver
{
    IReadOnlyList<IInterceptor> ResolveInterceptors(IInvocation invocation);
}
