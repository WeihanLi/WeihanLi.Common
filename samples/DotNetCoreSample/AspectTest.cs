using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using WeihanLi.Common;
using WeihanLi.Common.Aspect;
using WeihanLi.Common.DependencyInjection;
using WeihanLi.Extensions;

namespace DotNetCoreSample
{
    public class TestDbContext : DbContext
    {
        public TestDbContext(DbContextOptions<TestDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseSqlServer(DbConnectionString);
            optionsBuilder.UseInMemoryDatabase("Tests");
        }

        public DbSet<TestEntity> TestEntities { get; set; } = null!;
    }

    internal class DbContextSaveInterceptor : IInterceptor
    {
        public async Task Invoke(IInvocation invocation, Func<Task> next)
        {
            if (invocation.Target is DbContext dbContext)
            {
                dbContext.ChangeTracker.DetectChanges();
                foreach (var entry in dbContext.ChangeTracker.Entries())
                {
                    Console.WriteLine("---------------");
                    Console.WriteLine(entry.Entity.ToJson());
                    Console.WriteLine("---------------");
                }
            }
            if (invocation.ProxyTarget is DbContext dbContext1)
            {
                dbContext1.ChangeTracker.DetectChanges();
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
        public static void ServiceContainerTest()
        {
            var builder = new ServiceContainerBuilder();
            builder.AddSingletonProxy<IFly, MonkeyKing>();

            using var container = builder.BuildFluentAspectsContainer(options =>
            {
                options.InterceptAll()
                    .With<LogInterceptor>();
            });

            container.ResolveService<IFly>()
                .Fly();

            Console.WriteLine();
        }
    }
}
