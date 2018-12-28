using System.Data.SqlClient;
using WeihanLi.Common.Data;
using WeihanLi.Common.Helpers;

namespace DotNetCoreSample
{
    public static class RepositoryTest
    {
        public static void MainTest()
        {
            using (var conn = new SqlConnection(ConfigurationHelper.ConnectionString("TestDb")))
            {
                var repo = new Repository<TestEntity>(conn);
                // repo.Count(null);
                repo.Fetch(t => t.PKID == 10);
            }
        }
    }
}
