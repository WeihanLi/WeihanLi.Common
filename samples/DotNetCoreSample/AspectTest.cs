using System;
using System.Threading.Tasks;
using WeihanLi.Common.Aspect;

namespace DotNetCoreSample
{
    internal class TryInvokeInterceptor : IInterceptor
    {
        public async Task Invoke(IInvocation invocation, Func<Task> next)
        {
            try
            {
                Console.WriteLine("TryInvoke begin");
                await next();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                Console.WriteLine("TryInvoke end");
            }
        }
    }

    internal class AspectTest
    {
    }
}
