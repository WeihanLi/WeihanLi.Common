using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using Microsoft.Data.SqlClient;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;
using WeihanLi.Extensions;

namespace DotNetCoreSample;

public class DataExtensionTest
{
    /// <summary>
    /// Logger
    /// </summary>
    private static readonly ILogHelperLogger Logger = LogHelper.GetLogger<DataExtensionTest>();

    private static void Init(DbConnection conn)
    {
        var isTableExist = conn.ExecuteScalarToOrDefault<bool>(@"SELECT 1
            FROM dbo.sysobjects
            WHERE id = OBJECT_ID(N'[dbo].[TestTable111]')
            AND OBJECTPROPERTY(id, N'IsUserTable') = 1;");
        if (!isTableExist)
        {
            conn.Execute(@"CREATE TABLE [dbo].[TestTable111](
                    [PKID] [INT] IDENTITY(1, 1) PRIMARY KEY NOT NULL,

                    [Token] [NVARCHAR](200) NULL,

                    [CreatedTime] [DATETIME] NULL)");
        }
    }

    public static void MainTest()
    {
        var connString = DependencyResolver.ResolveRequiredService<IConfiguration>().GetConnectionString("TestDb");
        using var conn = new SqlConnection(connString);
        Init(conn);

        for (var i = 0; i < 3; i++)
        {
            conn.Execute(@"INSERT INTO [dbo].[TestTable111]
                            (
                            [Token],
                        [CreatedTime]
                        )
                    VALUES
                        (@Token, --Token - nvarchar(200)
                    @CreatedTime-- CreatedTime - datetime
                        )", new TestEntity
            {
                Token = Guid.NewGuid().ToString("N") + "_Execute_Model",
                CreatedTime = DateTime.Now
            });
        }

        Console.WriteLine("Current data count:{0}", conn.ExecuteScalarTo<int>("SELECT COUNT(1) FROM [dbo].[TestTable111]"));

        for (var i = 0; i < 3; i++)
        {
            conn.Execute(@"INSERT INTO [dbo].[TestTable111]
                            (
                            [Token],
                        [CreatedTime]
                        )
                    VALUES
                        (@Token, --Token - nvarchar(200)
                    GETDATE()-- CreatedTime - datetime
                        )", new TestEntity
            {
                Token = Guid.NewGuid().ToString("N") + "_Execute_Model_1"
            });
        }

        Console.WriteLine("Current data count:{0}", conn.ExecuteScalarTo<int>("SELECT COUNT(1) FROM [dbo].[TestTable111]"));

        for (var i = 0; i < 3; i++)
        {
            conn.Execute(@"INSERT INTO [dbo].[TestTable111]
                            (
                            [Token],
                        [CreatedTime]
                        )
                    VALUES
                        (@Token, --Token - nvarchar(200)
                    GETDATE()-- CreatedTime - datetime
                        )", new
            {
                Token = Guid.NewGuid().ToString("N") + "_Execute_Anonymous_Model"
            });
        }

        Console.WriteLine("Current data count:{0}", conn.ExecuteScalarTo<int>("SELECT COUNT(1) FROM [dbo].[TestTable111]"));

        var tokens = conn.QueryColumn<string>("SELECT Token FROM [dbo].[TestTable111]");
        Console.WriteLine("tokens:{0}", string.Join(",", tokens));

        var ids = conn.Select<int>("SELECT PKID FROM [dbo].[TestTable111]");
        Console.WriteLine("ids:{0}", string.Join(",", ids));

        var lastId = conn.Fetch<int>("SELECT TOP 1 PKID FROM [dbo].[TestTable111] ORDER BY PKID DESC");
        Console.WriteLine("lastId:{0}", lastId);

        conn.Execute("Delete from TestTable111 where PKID > @pkid", new { pkid = 888 });

        Console.WriteLine("Current data count:{0}", conn.ExecuteScalarTo<int>("SELECT COUNT(1) FROM [dbo].[TestTable111]"));

        Console.WriteLine(conn.Fetch<TestEntity>("select top 1 * from TestTable111")?.Token);

        foreach (var entity in conn.Select<TestEntity>("select * from TestTable111"))
        {
            Console.WriteLine(entity.Token);
        }

        Clean(conn);
    }

    private static void Clean(DbConnection conn)
    {
        conn.Execute("DROP TABLE [dbo].[TestTable111]");
    }
}

[Table("tabTestEntity")]
public class TestEntity
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("PKID")]
    public int Id { get; set; }

    public string? Token { get; set; }

    public DateTime CreatedTime { get; set; }
}
