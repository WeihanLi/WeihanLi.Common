using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WeihanLi.Common.Aspect;
using WeihanLi.Extensions;

namespace DotNetCoreSample
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<TestEntity> TestEntities { get; set; }
    }

    internal class DbContextSaveInterceptor : IInterceptor
    {
        public async Task Invoke(IInvocation invocation, Func<Task> next)
        {
            if (invocation.Target is DbContext dbContext)
            {
                foreach (var entry in dbContext.ChangeTracker.Entries())
                {
                    Console.WriteLine("---------------");
                    Console.WriteLine(entry.Entity.ToJson());
                    Console.WriteLine("---------------");
                }
            }
            if (invocation.ProxyTarget is DbContext dbContext1)
            {
                foreach (var entry in dbContext1.ChangeTracker.Entries())
                {
                    Console.WriteLine("---------------");
                    Console.WriteLine(entry.Entity.ToJson());
                    Console.WriteLine("---------------");
                }
            }
            Console.WriteLine($"{invocation.ProxyMethod.Name} before");
            await next();
            Console.WriteLine($"{invocation.ProxyMethod.Name} after");
        }
    }

    internal class AspectTest
    {
    }
}
