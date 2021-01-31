using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WeihanLi.Common.Aspect;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Test.AspectTest
{
    public class TestOutputInterceptor : IInterceptor
    {
        public async Task Invoke(IInvocation invocation, Func<Task> next)
        {
            Debug.WriteLine($"Method[{invocation.ProxyMethod.Name}({invocation.Arguments.StringJoin(",")})] is invoking...");
            await next();
            Debug.WriteLine($"Method[{invocation.ProxyMethod.Name}({invocation.Arguments.StringJoin(",")})] invoked...");
        }
    }
}
