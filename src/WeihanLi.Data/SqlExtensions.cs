using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using WeihanLi.Common;
using WeihanLi.Extensions;

namespace WeihanLi.Data
{
    public static class SqlExtensions
    {
        #region SqlConnection

        public static int BulkCopy<T>(this SqlConnection conn, IReadOnlyCollection<T> list, string tableName)
        {
            if (list == null || list.Count == 0)
            {
                return 0;
            }
            var props = CacheUtil.TypePropertyCache.GetOrAdd(typeof(T), t => t.GetProperties());
            var cols = conn.GetColumnNamesFromDb(tableName).Where(_ => props.Any(p => p.Name.EqualsIgnoreCase(_))).ToArray();
            var dataTable = new DataTable();
            dataTable.Columns.AddRange(cols.Select(c => new DataColumn(c)).ToArray());
            foreach (var item in list)
            {
                var row = dataTable.NewRow();
                foreach (DataColumn col in dataTable.Columns)
                {
                    row[col] = props.First(_ => _.Name.EqualsIgnoreCase(col.ColumnName)).GetValue(item);
                }
                dataTable.Rows.Add(row);
            }
            return conn.BulkCopy(dataTable, tableName);
        }

        public static async Task<int> BulkCopyAsync<T>(this SqlConnection conn, IReadOnlyCollection<T> list, string tableName)
        {
            if (list == null || list.Count == 0)
            {
                return 0;
            }
            var props = CacheUtil.TypePropertyCache.GetOrAdd(typeof(T), t => t.GetProperties());
            var cols = (await conn.GetColumnNamesFromDbAsync(tableName)).Where(_ => props.Any(p => p.Name.EqualsIgnoreCase(_))).ToArray();
            var dataTable = new DataTable();
            dataTable.Columns.AddRange(cols.Select(c => new DataColumn(c)).ToArray());
            foreach (var item in list)
            {
                var row = dataTable.NewRow();
                foreach (DataColumn col in dataTable.Columns)
                {
                    row[col] = props.First(_ => _.Name.EqualsIgnoreCase(col.ColumnName)).GetValue(item);
                }
                dataTable.Rows.Add(row);
            }
            return await conn.BulkCopyAsync(dataTable, tableName);
        }

        /// <summary>
        /// BulkCopy
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="dataTable">dataTable</param>
        /// <returns></returns>
        public static int BulkCopy(this SqlConnection conn, DataTable dataTable) => BulkCopy(conn, dataTable, dataTable.TableName);

        /// <summary>
        /// BulkCopy
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="dataTable">dataTable</param>
        /// <param name="destinationTableName">目标表</param>
        /// <returns></returns>
        public static int BulkCopy(this SqlConnection conn, DataTable dataTable, string destinationTableName) => BulkCopy(conn, dataTable, destinationTableName, 1000);

        /// <summary>
        /// BulkCopy
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="dataTable">dataTable</param>
        /// <param name="destinationTableName">目标表名称</param>
        /// <param name="batchSize">批量处理数量</param>
        /// <param name="columnMappings">columnMappings</param>
        /// <returns></returns>
        public static int BulkCopy(this SqlConnection conn, DataTable dataTable, string destinationTableName, int batchSize, IDictionary<string, string> columnMappings = null)
        {
            conn.EnsureOpen();
            using (var bulkCopy = new SqlBulkCopy(conn))
            {
                if (null == columnMappings)
                {
                    for (var i = 0; i < dataTable.Columns.Count; i++)
                    {
                        bulkCopy.ColumnMappings.Add(dataTable.Columns[i].ColumnName, dataTable.Columns[i].ColumnName);
                    }
                }
                else
                {
                    foreach (var columnMapping in columnMappings)
                    {
                        bulkCopy.ColumnMappings.Add(columnMapping.Key, columnMapping.Value);
                    }
                }

                bulkCopy.BatchSize = batchSize;
                bulkCopy.DestinationTableName = destinationTableName;
                bulkCopy.WriteToServer(dataTable);
                return 1;
            }
        }

        /// <summary>
        /// BulkCopyAsync
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="dataTable">dataTable</param>
        /// <returns></returns>
        public static Task<int> BulkCopyAsync(this SqlConnection conn, DataTable dataTable) => BulkCopyAsync(conn, dataTable, dataTable.TableName);

        /// <summary>
        /// BulkCopyAsync
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="dataTable">dataTable</param>
        /// <param name="destinationTableName">目标表</param>
        /// <returns></returns>
        public static Task<int> BulkCopyAsync(this SqlConnection conn, DataTable dataTable, string destinationTableName) => BulkCopyAsync(conn, dataTable, destinationTableName, 1000);

        /// <summary>
        /// BulkCopyAsync
        /// </summary>
        /// <param name="conn">数据库连接</param>
        /// <param name="dataTable">dataTable</param>
        /// <param name="destinationTableName">目标表名称</param>
        /// <param name="batchSize">批量处理数量</param>
        /// <param name="columnMappings">columnMappings</param>
        /// <returns></returns>
        public static async Task<int> BulkCopyAsync(this SqlConnection conn, DataTable dataTable, string destinationTableName, int batchSize, IDictionary<string, string> columnMappings = null)
        {
            await conn.EnsureOpenAsync();
            using (var bulkCopy = new SqlBulkCopy(conn))
            {
                if (null == columnMappings)
                {
                    for (var i = 0; i < dataTable.Columns.Count; i++)
                    {
                        bulkCopy.ColumnMappings.Add(dataTable.Columns[i].ColumnName, dataTable.Columns[i].ColumnName);
                    }
                }
                else
                {
                    foreach (var columnMapping in columnMappings)
                    {
                        bulkCopy.ColumnMappings.Add(columnMapping.Key, columnMapping.Value);
                    }
                }

                bulkCopy.BatchSize = batchSize;
                bulkCopy.DestinationTableName = destinationTableName;
                await bulkCopy.WriteToServerAsync(dataTable);
                return 1;
            }
        }

        #endregion SqlConnection
    }
}
