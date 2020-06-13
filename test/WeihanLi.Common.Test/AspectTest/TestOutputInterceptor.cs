using System;
using System.Threading.Tasks;
using WeihanLi.Common.Aspect;
using WeihanLi.Extensions;
using Xunit.Abstractions;

namespace WeihanLi.Common.Test.AspectTest
{
    public class TestOutputInterceptor : IInterceptor
    {
        private readonly ITestOutputHelper _output;

        public TestOutputInterceptor(ITestOutputHelper output)
        {
            _output = output;
        }

        public async Task Invoke(IInvocation invocation, Func<Task> next)
        {
            _output.WriteLine($"Method[{invocation.ProxyMethod.Name}({invocation.Arguments.StringJoin(",")})] is invoking...");
            await next();
            _output.WriteLine($"Method[{invocation.ProxyMethod.Name}({invocation.Arguments.StringJoin(",")})] invoked...");
        }
    }
}
