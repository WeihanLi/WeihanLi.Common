namespace WeihanLi.Common.Aspect;

[CLSCompliant(false)]
public sealed class DelegateInterceptor : AbstractInterceptor
{
    private readonly Func<IInvocation, Func<Task>, Task> _interceptFunc;

    public DelegateInterceptor(Func<IInvocation, Func<Task>, Task> interceptFunc)
    {
        _interceptFunc = Guard.NotNull(interceptFunc);
    }

    public override Task Invoke(IInvocation invocation, Func<Task> next)
    {
        return _interceptFunc.Invoke(invocation, next);
    }

    public static DelegateInterceptor FromDelegate(Func<IInvocation, Func<Task>, Task> interceptFunc) => new(interceptFunc);
}
