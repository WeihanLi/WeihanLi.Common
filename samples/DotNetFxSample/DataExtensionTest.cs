using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;
using WeihanLi.Extensions;

namespace DotNetFxSample
{
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
            using (var conn = new SqlConnection(ConfigurationHelper.ConnectionString("TestDb")))
            {
                Init(conn);
                var aaaa = conn.Fetch<TestEntity>("SELECT * FROM [dbo].[TestTable111]");

                for (int i = 0; i < 3; i++)
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

                var tokens = conn.QueryColumn<string>("SELECT Token FROM [dbo].[TestTable111]").ToArray();
                Console.WriteLine("tokens:{0}", string.Join(",", tokens));
                Console.WriteLine("Current data count:{0}", conn.ExecuteScalarTo<int>("SELECT COUNT(1) FROM [dbo].[TestTable111]"));

                for (int i = 0; i < 3; i++)
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
                    }, new SqlParameter("@Token", Guid.NewGuid().ToString("N")));
                }

                Console.WriteLine("Current data count:{0}", conn.ExecuteScalarTo<int>("SELECT COUNT(1) FROM [dbo].[TestTable111]"));

                for (int i = 0; i < 3; i++)
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

                conn.Execute(@"INSERT INTO [dbo].[TestTable111]
                            (
                            [Token],
                        [CreatedTime]
                        )
                    VALUES
                        (@Token, --Token - nvarchar(200)
                    GETDATE()-- CreatedTime - datetime
                        )", new Dictionary<string, object>
                {
                    { "@Token",Guid.NewGuid().ToString("N")+" executed-dictionary"}
                });

                Console.WriteLine("Current data count:{0}", conn.ExecuteScalarTo<int>("SELECT COUNT(1) FROM [dbo].[TestTable111]"));

                conn.Execute("Delete from TestTable111 where PKID > @pkid", new { pkid = 888 });

                Console.WriteLine("Current data count:{0}", conn.ExecuteScalarTo<int>("SELECT COUNT(1) FROM [dbo].[TestTable111]"));

                Clean(conn);
            }
        }

        public static void StorageProcusureTest()
        {
            using (var conn = new SqlConnection(ConfigurationHelper.ConnectionString("TestDb")))
            {
                //var result = conn.Execute("TestSP", CommandType.StoredProcedure, new { aaa = 123, bbb = "12333" });
                //var dataTable = conn.ExecuteDataTable("SELECT * FROM TestTable");

                try
                {
                    conn.Execute(@"INSERT INTO [dbo].[TestTable]
                            (
                            [Token],
                        [CreatedTime]
                        )
                    VALUES
                        (@Item1, --Token - nvarchar(200)
                    @Item2-- CreatedTime - datetime
                        )", GetValueTupleParam());
                }
                catch (NotSupportedException ex)
                {
                    Logger.Error(ex);
                }

                //conn.Execute(@"INSERT INTO [dbo].[TestTable]
                //            (
                //            [Token],
                //        [CreatedTime]
                //        )
                //    VALUES
                //        (@Token, --Token - nvarchar(200)
                //    @CreatedTime-- CreatedTime - datetime
                //        )", new TestStruct
                //{
                //    Token = Guid.NewGuid().ToString("N")+" execute-struct",
                //    CreatedTime = DateTime.Now
                //});

                var i = conn.ExecuteDataTable("SELECT * FROM TestTable").Rows.Count;
            }
        }

        private static void Clean(DbConnection conn)
        {
            conn.Execute("DROP TABLE [dbo].[TestTable111]");
        }

#if NET47
        private static (string Token, DateTime CreatedTime) GetValueTupleParam()
        {
            return ("token from valueTuple", DateTime.Now);
        }
#else

        private static Tuple<string, DateTime> GetValueTupleParam()
        {
            return Tuple.Create("token from valueTuple", DateTime.Now);
        }

#endif
    }

    internal class TestEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PKID { get; set; }

        public string Token { get; set; }

        public DateTime CreatedTime { get; set; }
    }

    internal struct TestStruct
    {
        public string Token { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
