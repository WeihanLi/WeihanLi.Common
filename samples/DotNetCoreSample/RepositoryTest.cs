using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.ObjectPool;
using System.Data.Common;
using System.Data.SqlClient;
using WeihanLi.Common;
using WeihanLi.Common.Data;
using WeihanLi.Extensions;

namespace DotNetCoreSample;

public static class RepositoryTest
{
    public static void MainTest()
    {
        var connectionPool = new DbConnectionPool(new DbConnectionPoolPolicy(DependencyResolver.ResolveRequiredService<IConfiguration>()
            .GetRequiredConnectionString("Test")!));

        var repo = new Repository<TestEntity>(() => connectionPool.Get());
        repo.Execute("TRUNCATE TABLE dbo.tabTestEntity");

        repo.Insert(new TestEntity
        {
            Token = "1233",
            CreatedTime = DateTime.UtcNow
        });

        var entity = repo.Fetch(t => t.Id == 1);
        System.Console.WriteLine(entity?.Token);

        repo.Update(t => t.Id == 1, t => t.Token, 1);

        entity = repo.Fetch(t => t.Id == 1);
        System.Console.WriteLine(entity?.Token);

        var exists = repo.Exist(e => e.Id == 1);
        Console.WriteLine($"exists pkid == 1: {exists}");

        repo.Delete(t => t.Id == 1);
        entity = repo.Fetch(t => t.Id == 1);
        System.Console.WriteLine($"delete operation {(entity == null ? "Success" : "Failed")}");

        exists = repo.Exist(e => e.Id > 1000);
        Console.WriteLine($"exists PKID > 1000: {exists}");
        repo.Execute("TRUNCATE TABLE dbo.tabTestEntity");

        var data = repo.Paged(1, 10, t => t.Id > 10, t => t.CreatedTime, true);
        Console.WriteLine($"TotalCount: {data.TotalCount}, dataCount: {data.Count}");

        Console.WriteLine("finished.");
    }

    public class DbConnectionPool : DefaultObjectPool<DbConnection>
    {
        public DbConnectionPool(IPooledObjectPolicy<DbConnection> policy) : base(policy)
        {
        }

        public DbConnectionPool(IPooledObjectPolicy<DbConnection> policy, int maximumRetained) : base(policy, maximumRetained)
        {
        }
    }

    public class DbConnectionPoolPolicy : IPooledObjectPolicy<DbConnection>
    {
        private readonly string _connString;

        public DbConnectionPoolPolicy(string connString)
        {
            _connString = connString;
        }

        public DbConnection Create()
        {
            return new SqlConnection(_connString);
        }

        public bool Return(DbConnection obj)
        {
            return obj.ConnectionString.IsNotNullOrWhiteSpace();
        }
    }
}
