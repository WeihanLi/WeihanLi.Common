using System;
using System.Threading.Tasks;

namespace WeihanLi.Common.Aspect
{
    public sealed class DelegateInterceptor : AbstractInterceptor
    {
        private readonly Func<IInvocation, Func<Task>, Task> _interceptFunc;

        public DelegateInterceptor(Func<IInvocation, Func<Task>, Task> interceptFunc)
        {
            _interceptFunc = interceptFunc ?? throw new ArgumentNullException(nameof(interceptFunc));
        }

        public override Task Invoke(IInvocation invocation, Func<Task> next)
        {
            return _interceptFunc.Invoke(invocation, next);
        }

        public static DelegateInterceptor FromDelegate(Func<IInvocation, Func<Task>, Task> interceptFunc) => new DelegateInterceptor(interceptFunc);
    }
}
