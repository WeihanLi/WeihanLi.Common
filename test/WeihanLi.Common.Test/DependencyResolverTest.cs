using Microsoft.Extensions.DependencyInjection;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using WeihanLi.Common.Data;
using WeihanLi.Common.Helpers;
using Xunit;

namespace WeihanLi.Common.Test
{
    public class DependencyResolverTest
    {
        private const string DbConnectionString =
          @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Test;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        [Fact]
        public async Task ScopeTest()
        {
            var serviceHashCodes = new ConcurrentSet<int>();
            var services = new ServiceCollection();
            services.AddSingleton<Func<DbConnection>>(x => () => new SqlConnection(DbConnectionString));
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            DependencyResolver.SetDependencyResolver(services);

            DependencyResolver.TryInvokeService<IRepository<TestEntity>>(repo =>
                {
                    serviceHashCodes.Add(repo.GetHashCode());
                });
            await Task.WhenAll(

                DependencyResolver.TryInvokeServiceAsync<IRepository<TestEntity>>(repo =>
                {
                    serviceHashCodes.Add(repo.GetHashCode());
                    return Task.CompletedTask;
                }),
                DependencyResolver.TryInvokeServiceAsync<IRepository<TestEntity>>(repo =>
                {
                    serviceHashCodes.Add(repo.GetHashCode());
                    return Task.CompletedTask;
                })
            );

            Assert.Equal(3, serviceHashCodes.Count);
        }
    }

    [Table("TestEntities")]
    internal class TestEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Extra { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }
}
