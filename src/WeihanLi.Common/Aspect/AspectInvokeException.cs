namespace WeihanLi.Common.Aspect;

public sealed class AspectInvokeException(IInvocation invocation, Exception innerException) : Exception($"Invoke {invocation.ProxyMethod.Name} exception", innerException)
{
    public IInvocation Invocation { get; } = invocation;
}
