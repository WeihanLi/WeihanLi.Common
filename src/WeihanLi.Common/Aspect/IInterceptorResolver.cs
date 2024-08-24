namespace WeihanLi.Common.Aspect;

public interface IInterceptorResolver
{
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    IReadOnlyList<IInterceptor> ResolveInterceptors(IInvocation invocation);
}
