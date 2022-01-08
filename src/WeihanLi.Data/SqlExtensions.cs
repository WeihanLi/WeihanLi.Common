using System.Data;
using Microsoft.Data.SqlClient;
using WeihanLi.Common;
using WeihanLi.Extensions;

namespace WeihanLi.Data;

public static class SqlExtensions
{
    private const string QueryTableColumnsSqlText = @"
SELECT c.[name]
FROM sys.columns c
    JOIN sys.tables t
        ON c.object_id = t.object_id
WHERE t.name = @tableName
ORDER BY c.[column_id];
";

    #region SqlConnection

    /// <summary>
    /// 从数据库中根据表名获取列名
    /// </summary>
    /// <param name="connection">数据库连接</param>
    /// <param name="tableName">表名称</param>
    /// <returns></returns>
    public static IEnumerable<string> GetColumnNamesFromDb(this SqlConnection connection, string tableName)
        => connection.QueryColumn<string>(QueryTableColumnsSqlText, new { tableName });

    /// <summary>
    /// 从数据库中根据表名获取列名
    /// </summary>
    /// <param name="connection">数据库连接</param>
    /// <param name="tableName">表名称</param>
    /// <returns></returns>
    public static Task<IEnumerable<string>> GetColumnNamesFromDbAsync(this SqlConnection connection, string tableName)
        => connection.QueryColumnAsync<string>(QueryTableColumnsSqlText, new { tableName });

    public static int BulkCopy<T>(this SqlConnection conn, IReadOnlyCollection<T> list, string tableName) => BulkCopy(conn, list, tableName, 60);

    public static int BulkCopy<T>(this SqlConnection conn, IReadOnlyCollection<T>? list, string tableName, int bulkCopyTimeout)
    {
        if (list == null || list.Count == 0)
        {
            return 0;
        }
        var props = CacheUtil.GetTypeFields(typeof(T));
        var cols = conn.GetColumnNamesFromDb(tableName).Where(_ => props.Any(p => p.Name.EqualsIgnoreCase(_))).ToArray();
        var dataTable = new DataTable();
        dataTable.Columns.AddRange(cols.Select(c => new DataColumn(c)).ToArray());
        foreach (var item in list)
        {
            var row = dataTable.NewRow();
            foreach (DataColumn col in dataTable.Columns)
            {
                row[col] = props.FirstOrDefault(_ => _.Name.EqualsIgnoreCase(col.ColumnName))
                    ?.GetValue(item);
            }
            dataTable.Rows.Add(row);
        }
        return conn.BulkCopy(dataTable, tableName, bulkCopyTimeout);
    }

    /// <summary>
    /// BulkCopy
    /// </summary>
    /// <param name="conn">数据库连接</param>
    /// <param name="dataTable">dataTable</param>
    /// <returns></returns>
    public static int BulkCopy(this SqlConnection conn, DataTable dataTable) => BulkCopy(conn, dataTable, 60);

    public static int BulkCopy(this SqlConnection conn, DataTable dataTable, int bulkCopyTimeout) => BulkCopy(conn, dataTable, dataTable.TableName, bulkCopyTimeout);

    /// <summary>
    /// BulkCopy
    /// </summary>
    /// <param name="conn">数据库连接</param>
    /// <param name="dataTable">dataTable</param>
    /// <param name="destinationTableName">目标表</param>
    /// <returns></returns>
    public static int BulkCopy(this SqlConnection conn, DataTable dataTable, string destinationTableName) => BulkCopy(conn, dataTable, destinationTableName, 60);

    public static int BulkCopy(this SqlConnection conn, DataTable dataTable, string destinationTableName, int bulkCopyTimeout) => BulkCopy(conn, dataTable, destinationTableName, 1000, null, bulkCopyTimeout: bulkCopyTimeout);

    /// <summary>
    /// BulkCopy
    /// </summary>
    /// <param name="conn">数据库连接</param>
    /// <param name="dataTable">dataTable</param>
    /// <param name="destinationTableName">目标表名称</param>
    /// <param name="batchSize">批量处理数量</param>
    /// <param name="columnMappings">columnMappings</param>
    /// <param name="bulkCopyTimeout">bulkCopyTimeout</param>
    /// <returns></returns>
    public static int BulkCopy(this SqlConnection conn, DataTable dataTable, string destinationTableName, int batchSize, IDictionary<string, string>? columnMappings, int bulkCopyTimeout = 60)
    {
        conn.EnsureOpen();
        using var bulkCopy = new SqlBulkCopy(conn);
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
        bulkCopy.BulkCopyTimeout = bulkCopyTimeout;
        bulkCopy.DestinationTableName = destinationTableName;
        bulkCopy.WriteToServer(dataTable);
        return 1;
    }

    public static Task<int> BulkCopyAsync<T>(this SqlConnection conn, IReadOnlyCollection<T> list, string tableName) => BulkCopyAsync(conn, list, tableName, 60);

    public static async Task<int> BulkCopyAsync<T>(this SqlConnection conn, IReadOnlyCollection<T>? list, string tableName, int bulkCopyTimeout)
    {
        if (list == null || list.Count == 0)
        {
            return 0;
        }
        var props = CacheUtil.GetTypeProperties(typeof(T));
        var cols = (await conn.GetColumnNamesFromDbAsync(tableName)).Where(_ => props.Any(p => p.Name.EqualsIgnoreCase(_))).ToArray();
        var dataTable = new DataTable();
        dataTable.Columns.AddRange(cols.Select(c => new DataColumn(c)).ToArray());
        foreach (var item in list)
        {
            var row = dataTable.NewRow();
            foreach (DataColumn col in dataTable.Columns)
            {
                row[col] = props.FirstOrDefault(_ => _.Name.EqualsIgnoreCase(col.ColumnName))?.GetValue(item);
            }
            dataTable.Rows.Add(row);
        }
        return await conn.BulkCopyAsync(dataTable, tableName, bulkCopyTimeout);
    }

    /// <summary>
    /// BulkCopyAsync
    /// </summary>
    /// <param name="conn">数据库连接</param>
    /// <param name="dataTable">dataTable</param>
    /// <returns></returns>
    public static Task<int> BulkCopyAsync(this SqlConnection conn, DataTable dataTable) => BulkCopyAsync(conn, dataTable, dataTable.TableName, 60);

    public static Task<int> BulkCopyAsync(this SqlConnection conn, DataTable dataTable, int bulkCopyTimeout) => BulkCopyAsync(conn, dataTable, dataTable.TableName);

    /// <summary>
    /// BulkCopyAsync
    /// </summary>
    /// <param name="conn">数据库连接</param>
    /// <param name="dataTable">dataTable</param>
    /// <param name="destinationTableName">目标表</param>
    /// <returns></returns>
    public static Task<int> BulkCopyAsync(this SqlConnection conn, DataTable dataTable, string destinationTableName) => BulkCopyAsync(conn, dataTable, destinationTableName, bulkCopyTimeout: 60);

    public static Task<int> BulkCopyAsync(this SqlConnection conn, DataTable dataTable, string destinationTableName, int bulkCopyTimeout) => BulkCopyAsync(conn, dataTable, destinationTableName, 1000, null, bulkCopyTimeout: bulkCopyTimeout);

    /// <summary>
    /// BulkCopyAsync
    /// </summary>
    /// <param name="conn">数据库连接</param>
    /// <param name="dataTable">dataTable</param>
    /// <param name="destinationTableName">目标表名称</param>
    /// <param name="batchSize">批量处理数量</param>
    /// <param name="columnMappings">columnMappings</param>
    /// <param name="bulkCopyTimeout">bulkCopyTimeout</param>
    /// <returns></returns>
    public static async Task<int> BulkCopyAsync(this SqlConnection conn, DataTable dataTable, string destinationTableName, int batchSize, IDictionary<string, string>? columnMappings, int bulkCopyTimeout = 60)
    {
        await conn.EnsureOpenAsync();
        using var bulkCopy = new SqlBulkCopy(conn);
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
        bulkCopy.BulkCopyTimeout = bulkCopyTimeout;
        bulkCopy.DestinationTableName = destinationTableName;
        await bulkCopy.WriteToServerAsync(dataTable);
        return 1;
    }

    #endregion SqlConnection
}
