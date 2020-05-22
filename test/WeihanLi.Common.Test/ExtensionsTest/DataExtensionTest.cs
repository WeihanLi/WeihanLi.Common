using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WeihanLi.Common.Data;
using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test.ExtensionsTest
{
    [Table("TestEntities")]
    internal class TestEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Extra { get; set; }

        public DateTimeOffset CreatedAt { get; set; }
    }

    public class DataExtensionTest : IDisposable
    {
        private const string DbConnectionString =
            @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=TestDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

        private readonly SqlConnection _connection = null;

        public DataExtensionTest()
        {
            _connection = new SqlConnection(DbConnectionString);

            //var _repository = new Repository<TestEntity>(() => _connection);

            //_repository.Insert(new TestEntity() { Extra = "abc11", CreatedAt = DateTime.Now, });
            //_repository.Insert(new TestEntity() { Extra = "abc22", CreatedAt = DateTime.Now, });
            //_repository.Insert(new TestEntity() { Extra = "abc33", CreatedAt = DateTime.Now, });
        }

        [Fact(
        Skip = "DbExtensionTest"
        )]
        public void FetchTest()
        {
            var entity = _connection.Fetch<TestEntity>("SELECT TOP 1 * FROM TestEntities");
            Assert.NotNull(entity);
        }

        [Fact(
        Skip = "DbExtensionTest"
        )]
        public async Task FetchAsyncTest()
        {
            var entity = await _connection.FetchAsync<TestEntity>("SELECT TOP 1 * FROM TestEntities");
            Assert.NotNull(entity);
        }

        [Fact(
        Skip = "DbExtensionTest"
        )]
        public void SelectTest()
        {
            var result = _connection.Select<TestEntity>("SELECT * FROM TestEntities").ToArray();
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact(
        Skip = "DbExtensionTest"
        )]
        public async Task SelectAsyncTest()
        {
            var result = (await _connection.SelectAsync<TestEntity>("SELECT * FROM TestEntities")).ToArray();
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact(
        Skip = "DbExtensionTest"
        )]
        public void SelectColumnTest()
        {
            var result = _connection.SelectColumn<int>("SELECT * FROM TestEntities").ToArray();
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        [Fact(
        Skip = "DbExtensionTest"
        )]
        public async Task SelectColumnAsyncTest()
        {
            var result = (await _connection.SelectColumnAsync<int>("SELECT * FROM TestEntities")).ToArray();
            Assert.NotNull(result);
            Assert.NotEmpty(result);
        }

        public void Dispose() => _connection?.Dispose();
    }
}
