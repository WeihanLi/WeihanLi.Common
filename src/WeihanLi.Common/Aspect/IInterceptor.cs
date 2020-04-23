using System;
using System.Threading.Tasks;

namespace WeihanLi.Common.Aspect
{
    public interface IInterceptor
    {
        Task Invoke(IInvocation invocation, Func<Task> next);
    }
}
