using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WeihanLi.Common.Aspect;

namespace DotNetCoreSample
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }
    }

    internal class DbContextSaveInterceptor : IInterceptor
    {
        public async Task Invoke(IInvocation invocation, Func<Task> next)
        {
            Console.WriteLine($"{invocation.ProxyMethod.Name} before");
            await next();
            Console.WriteLine($"{invocation.ProxyMethod.Name} after");
        }
    }

    internal class AspectTest
    {
    }
}
