using System;
using System.Data.Common;
using System.Data.SqlClient;
using Microsoft.Extensions.ObjectPool;
using WeihanLi.Common.Data;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace DotNetFxSample
{
    public static class RepositoryTest
    {
        public static void MainTest()
        {
            var connectionPool = new DbConnectionPool(new DbConnectionPoolPolicy(ConfigurationHelper.ConnectionString("TestDb")));

            var repo = new Repository<TestEntity>(() => connectionPool.Get());
            repo.Insert(new TestEntity
            {
                Token = "1233",
                CreatedTime = DateTime.UtcNow
            });

            var entity = repo.Fetch(t => t.PKID == 1);
            System.Console.WriteLine(entity.Token);

            repo.Update(t => t.PKID == 1, t => t.Token, 1);

            entity = repo.Fetch(t => t.PKID == 1);
            System.Console.WriteLine(entity.Token);

            repo.Delete(t => t.PKID == 1);
            entity = repo.Fetch(t => t.PKID == 1);
            System.Console.WriteLine($"delete operation {(entity == null ? "Success" : "Failed")}");

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
}
