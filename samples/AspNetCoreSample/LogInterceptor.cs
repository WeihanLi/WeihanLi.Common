using System;
using System.Diagnostics;
using System.Threading.Tasks;
using WeihanLi.Common.Aspect;
using WeihanLi.Extensions;

namespace AspNetCoreSample
{
    public class EventPublishLogInterceptor : AbstractInterceptor
    {
        public override async Task Invoke(IInvocation invocation, Func<Task> next)
        {
            Console.WriteLine("-------------------------------");
            Console.WriteLine($"Event publish begin, eventData:{invocation.Arguments.ToJson()}");
            var watch = Stopwatch.StartNew();
            try
            {
                await next();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Event publish exception({ex})");
            }
            finally
            {
                watch.Stop();
                Console.WriteLine($"Event publish complete, elasped:{watch.ElapsedMilliseconds} ms");
            }
            Console.WriteLine("-------------------------------");
        }
    }
}
