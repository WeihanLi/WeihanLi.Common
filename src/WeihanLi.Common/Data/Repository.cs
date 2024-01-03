using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using WeihanLi.Common.Models;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Data;

[CLSCompliant(false)]
public class Repository<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors | DynamicallyAccessedMemberTypes.PublicProperties)] TEntity> : IRepository<TEntity> where TEntity : new()
{
    #region TODO: Cache External

    private readonly Lazy<Dictionary<string, string>> PrimaryKeyColumns = new(() =>
              CacheUtil.GetTypeProperties(typeof(TEntity))
              .Any(x => x.IsDefined(typeof(KeyAttribute)))
              ?
              CacheUtil.GetTypeProperties(typeof(TEntity))
              .ToDictionary(x => x.Name, x => x.GetColumnName())
              :
                  new Dictionary<string, string>(1)
                  {
                    { "Id",
                        CacheUtil.GetTypeProperties(typeof(TEntity))
                        .FirstOrDefault(x => x.Name.Equals("Id"))?.GetColumnName()
                        ?? throw new InvalidOperationException("no primary key found")
                        }
                  }
            )
        ;

    private readonly Dictionary<string, string> ColumnMappings = CacheUtil.GetTypeProperties(typeof(TEntity))
         .Where(_ => !_.IsDefined(typeof(NotMappedAttribute)))
         .Select(p => new KeyValuePair<string, string>(p.GetColumnName(), p.Name))
         .ToDictionary(p => p.Key, p => p.Value);

    private readonly string SelectColumnsString = CacheUtil.GetTypeProperties(typeof(TEntity))
        .Where(_ => !_.IsDefined(typeof(NotMappedAttribute))).Select(_ => $"{_.GetColumnName()} AS {_.Name}").StringJoin(",");

    private readonly Lazy<Dictionary<string, string>> InsertColumnMappings = new(() => CacheUtil.GetTypeProperties(typeof(TEntity))
              .Where(_ => !_.IsDefined(typeof(NotMappedAttribute)) && !_.IsDefined(typeof(DatabaseGeneratedAttribute)))
              .Select(p => new KeyValuePair<string, string>(p.GetColumnName(), p.Name))
              .ToDictionary(_ => _.Key, _ => _.Value));

    private readonly string _tableName = typeof(TEntity).GetCustomAttribute<TableAttribute>()?.Name ?? typeof(TEntity).Name;

    #endregion TODO: Cache External

    protected readonly Lazy<DbConnection> _dbConnection;

    public Repository(Func<DbConnection> dbConnectionFunc)
    {
        _dbConnection = new Lazy<DbConnection>(dbConnectionFunc, true);
    }

    public virtual int Count(Expression<Func<TEntity, bool>> whereExpression)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);

        var sql = $@"
SELECT COUNT(1) FROM {_tableName}
{whereSql.SqlText}
";
        return _dbConnection.Value.ExecuteScalarTo<int>(sql, whereSql.Parameters);
    }

    public virtual Task<int> CountAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);

        var sql = $@"
SELECT COUNT(1) FROM {_tableName}
{whereSql.SqlText}
";
        return _dbConnection.Value.ExecuteScalarToAsync<int>(sql, whereSql.Parameters, cancellationToken: cancellationToken);
    }

    public virtual long LongCount(Expression<Func<TEntity, bool>> whereExpression)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);

        var sql = $@"
SELECT COUNT(1) FROM {_tableName}
{whereSql.SqlText}
";
        return _dbConnection.Value.ExecuteScalarTo<long>(sql, whereSql.Parameters);
    }

    public virtual Task<long> LongCountAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);

        var sql = $@"
SELECT COUNT(1) FROM {_tableName}
{whereSql.SqlText}
";
        return _dbConnection.Value.ExecuteScalarToAsync<long>(sql, whereSql.Parameters, cancellationToken: cancellationToken);
    }

    public virtual bool Exist(Expression<Func<TEntity, bool>> whereExpression)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);
        var sql = $@"SELECT CAST(IIF(EXISTS (SELECT TOP(1) 1 FROM {_tableName} {whereSql.SqlText}), 1, 0) AS BIT)";
        return _dbConnection.Value.ExecuteScalarTo<bool>(sql, whereSql.Parameters);
    }

    public virtual Task<bool> ExistAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);
        var sql = $@"SELECT CAST(IIF(EXISTS (SELECT TOP(1) 1 FROM {_tableName} {whereSql.SqlText}), 1, 0) AS BIT)";
        return _dbConnection.Value.ExecuteScalarToAsync<bool>(sql, whereSql.Parameters, cancellationToken: cancellationToken);
    }

    public virtual TEntity? Fetch(Expression<Func<TEntity, bool>> whereExpression)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);
        var sql = $@"
SELECT TOP(1) {SelectColumnsString} FROM {_tableName}
{whereSql.SqlText}
";
        return _dbConnection.Value.Fetch<TEntity>(sql, whereSql.Parameters);
    }

    public virtual Task<TEntity?> FetchAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);
        var sql = $@"
SELECT TOP 1 {SelectColumnsString} FROM {_tableName}
{whereSql.SqlText}
";
        return _dbConnection.Value.FetchAsync<TEntity>(sql, whereSql.Parameters, cancellationToken: cancellationToken);
    }

    public virtual TEntity? Fetch<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);
        var sql = $@"
SELECT TOP(1) {SelectColumnsString} FROM {_tableName}
{whereSql.SqlText}
ORDER BY {GetColumnName(orderByExpression.GetMemberName())}  {(ascending ? "" : "DESC")}
";
        return _dbConnection.Value.Fetch<TEntity>(sql, whereSql.Parameters);
    }

    public virtual Task<TEntity?> FetchAsync<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false, CancellationToken cancellationToken = default)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);
        var sql = $@"
SELECT TOP(1) {SelectColumnsString} FROM {_tableName}
{whereSql.SqlText}
ORDER BY {GetColumnName(orderByExpression.GetMemberName())}  {(ascending ? "" : "DESC")}
";
        return _dbConnection.Value.FetchAsync<TEntity>(sql, whereSql.Parameters, cancellationToken: cancellationToken);
    }

    public virtual List<TEntity> Select(Expression<Func<TEntity, bool>> whereExpression)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);
        var sql = $@"
SELECT {SelectColumnsString} FROM {_tableName}
{whereSql.SqlText}
";
        return _dbConnection.Value.Select<TEntity>(sql, whereSql.Parameters).ToList();
    }

    public virtual Task<List<TEntity>> SelectAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);
        var sql = $@"
SELECT {SelectColumnsString} FROM {_tableName}
{whereSql.SqlText}
";
        return _dbConnection.Value.SelectAsync<TEntity>(sql, whereSql.Parameters, cancellationToken: cancellationToken).ContinueWith(r => r.Result.ToList(), cancellationToken);
    }

    public virtual List<TEntity> Select<TProperty>(int count, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);
        var sql = $@"
SELECT TOP({count}) {SelectColumnsString} FROM {_tableName}
{whereSql.SqlText}
ORDER BY {GetColumnName(orderByExpression.GetMemberName())} {(ascending ? "" : "DESC")}
";
        return _dbConnection.Value.Select<TEntity>(sql, whereSql.Parameters).ToList();
    }

    public virtual Task<List<TEntity>> SelectAsync<TProperty>(int count, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false, CancellationToken cancellationToken = default)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);
        var sql = $@"
SELECT TOP({count}) {SelectColumnsString} FROM {_tableName}
{whereSql.SqlText}
ORDER BY {GetColumnName(orderByExpression.GetMemberName())} {(ascending ? "" : "DESC")}
";
        return _dbConnection.Value.SelectAsync<TEntity>(sql, whereSql.Parameters, cancellationToken: cancellationToken).ContinueWith(_ => _.Result.ToList(), cancellationToken);
    }

    public virtual IPagedListResult<TEntity> Paged<TProperty>(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);
        if (pageNumber <= 0)
        {
            pageNumber = 1;
        }
        if (pageSize <= 0)
        {
            pageSize = 10;
        }
        var sql = $@"
SELECT COUNT(1) FROM {_tableName}
{whereSql.SqlText}
";
        var total = _dbConnection.Value.ExecuteScalarTo<int>(sql, whereSql.Parameters);
        if (total == 0)
        {
            return PagedListResult<TEntity>.Empty;
        }

        var offset = (pageNumber - 1) * pageSize;

        sql = $@"
SELECT {SelectColumnsString} FROM {_tableName}
{whereSql.SqlText}
ORDER BY {GetColumnName(orderByExpression.GetMemberName())}{(ascending ? "" : " DESC")}
OFFSET {offset} ROWS
FETCH NEXT {pageSize} ROWS ONLY
";

        return _dbConnection.Value.Select<TEntity>(sql, whereSql.Parameters).ToPagedList(pageNumber, pageSize, total);
    }

    public virtual async Task<IPagedListResult<TEntity>> PagedAsync<TProperty>(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false, CancellationToken cancellationToken = default)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);
        if (pageNumber <= 0)
        {
            pageNumber = 1;
        }
        if (pageSize <= 0)
        {
            pageSize = 10;
        }
        var sql = $@"
SELECT COUNT(1) FROM {_tableName}
{whereSql.SqlText}
";
        var total = await _dbConnection.Value.ExecuteScalarToAsync<int>(sql, whereSql.Parameters, cancellationToken: cancellationToken);
        if (total == 0)
        {
            return PagedListResult<TEntity>.Empty;
        }

        var offset = (pageNumber - 1) * pageSize;

        sql = $@"
SELECT {SelectColumnsString} FROM {_tableName}
{whereSql.SqlText}
ORDER BY {GetColumnName(orderByExpression.GetMemberName())}{(ascending ? "" : " DESC")}
OFFSET {offset} ROWS
FETCH NEXT {pageSize} ROWS ONLY
";

        return (await _dbConnection.Value.SelectAsync<TEntity>(sql, whereSql.Parameters, cancellationToken: cancellationToken)).ToPagedList(pageNumber, pageSize, total);
    }

    public virtual int Insert(TEntity entity)
    {
        var paramDictionary = new Dictionary<string, object?>();
        var sqlBuilder = new StringBuilder($@"INSERT INTO {_tableName}");
        sqlBuilder.AppendLine();
        sqlBuilder.AppendLine("(");
        sqlBuilder.AppendLine($"{InsertColumnMappings.Value.Keys.Select(_ => _).StringJoin($",{Environment.NewLine}")}");
        sqlBuilder.AppendLine(")");
        sqlBuilder.AppendLine("VALUES");
        sqlBuilder.AppendLine("(");
        sqlBuilder.AppendLine($"{InsertColumnMappings.Value.Keys.Select(_ => $"@{_}").StringJoin($",{Environment.NewLine}")}");
        sqlBuilder.AppendLine(")");
        foreach (var field in InsertColumnMappings.Value.Keys)
        {
            paramDictionary.Add($"{field}", Guard.NotNull(typeof(TEntity).GetProperty(InsertColumnMappings.Value[field])).GetValue(entity));
        }
        var sql = sqlBuilder.ToString();

        return _dbConnection.Value.Execute(sql, paramDictionary);
    }

    public virtual Task<int> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var paramDictionary = new Dictionary<string, object?>();

        var sqlBuilder = new StringBuilder($@"INSERT INTO {_tableName}");
        sqlBuilder.AppendLine("(");
        sqlBuilder.AppendLine($"{InsertColumnMappings.Value.Keys.Select(_ => _).StringJoin($",{Environment.NewLine}")}");
        sqlBuilder.AppendLine(")");
        sqlBuilder.AppendLine("VALUES (");
        sqlBuilder.AppendLine($"{InsertColumnMappings.Value.Keys.Select(_ => $"@{_}").StringJoin($",{Environment.NewLine}")}");
        foreach (var field in InsertColumnMappings.Value.Keys)
        {
            paramDictionary.Add($"{field}", Guard.NotNull(typeof(TEntity).GetProperty(InsertColumnMappings.Value[field])).GetValue(entity));
        }

        sqlBuilder.AppendLine(")");
        return _dbConnection.Value.ExecuteAsync(sqlBuilder.ToString(), paramDictionary, cancellationToken: cancellationToken);
    }

    public virtual int Insert(IEnumerable<TEntity> entities)
    {
        var count = entities.Count();
        if (count == 0)
        {
            return 0;
        }
        if (count > 1000)
        {
            return -1; // too large, not supported
        }
        var paramDictionary = new Dictionary<string, object?>();
        var sqlBuilder = new StringBuilder($@"INSERT INTO {_tableName}");
        sqlBuilder.AppendLine("(");
        sqlBuilder.AppendLine($"{InsertColumnMappings.Value.Keys.Select(_ => _).StringJoin($",{Environment.NewLine}")}");
        sqlBuilder.AppendLine(")");
        sqlBuilder.AppendLine("VALUES");

        foreach (var entity in entities)
        {
            var i = 0;
            sqlBuilder.AppendLine();
            sqlBuilder.AppendLine("(");
            sqlBuilder.AppendLine($"{InsertColumnMappings.Value.Keys.Select(_ => $"@{_}_{i}").StringJoin($",{Environment.NewLine}")}");
            foreach (var field in InsertColumnMappings.Value.Keys)
            {
                paramDictionary.Add($"{field}_{i}", Guard.NotNull(typeof(TEntity).GetProperty(InsertColumnMappings.Value[field])).GetValue(entity));
            }
            sqlBuilder.Append("),");
            i++;
        }
        sqlBuilder.Remove(sqlBuilder.Length - 2, 1);

        return _dbConnection.Value.Execute(sqlBuilder.ToString(), paramDictionary);
    }

    public virtual Task<int> InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        var count = entities.Count();
        if (count == 0)
        {
            return Task.FromResult(0);
        }
        if (count > 1000)
        {
            return Task.FromResult(-1); // too large, not supported
        }
        var paramDictionary = new Dictionary<string, object?>();
        var sqlBuilder = new StringBuilder($@"INSERT INTO {_tableName}");
        sqlBuilder.AppendLine("(");
        sqlBuilder.AppendLine($"{InsertColumnMappings.Value.Keys.Select(_ => _).StringJoin($",{Environment.NewLine}")}");
        sqlBuilder.AppendLine(")");
        sqlBuilder.AppendLine("VALUES");

        foreach (var entity in entities)
        {
            var i = 0;
            sqlBuilder.AppendLine();
            sqlBuilder.AppendLine("(");
            sqlBuilder.AppendLine($"{InsertColumnMappings.Value.Keys.Select(_ => $"@{_}_{i}").StringJoin($",{Environment.NewLine}")}");
            foreach (var field in InsertColumnMappings.Value.Keys)
            {
                paramDictionary.Add($"{field}_{i}", Guard.NotNull(typeof(TEntity).GetProperty(InsertColumnMappings.Value[field])).GetValue(entity));
            }
            sqlBuilder.Append("),");
            i++;
        }

        sqlBuilder.Remove(sqlBuilder.Length - 2, 1);

        return _dbConnection.Value.ExecuteAsync(sqlBuilder.ToString(), paramDictionary, cancellationToken: cancellationToken);
    }

    public virtual int Update<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> propertyExpression, object? value)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);
        var propertyName = propertyExpression.GetMemberName();
        var sql = $@"
UPDATE {_tableName}
SET {GetColumnName(propertyName)} = @set_{propertyName}
{whereSql.SqlText}
";
        whereSql.Parameters.Add($"set_{propertyName}", value);
        return _dbConnection.Value.Execute(sql, whereSql.Parameters);
    }

    public virtual Task<int> UpdateAsync<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> propertyExpression, object? value, CancellationToken cancellationToken = default)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);
        var propertyName = propertyExpression.GetMemberName();
        var sql = $@"
UPDATE {_tableName}
SET {GetColumnName(propertyName)} = @set_{propertyName}
{whereSql.SqlText}
";
        whereSql.Parameters.Add($"set_{propertyName}", value);
        return _dbConnection.Value.ExecuteAsync(sql, whereSql.Parameters, cancellationToken: cancellationToken);
    }

    public virtual int Update(Expression<Func<TEntity, bool>> whereExpression, IDictionary<string, object?> propertyValues)
    {
        if (propertyValues.IsNullOrEmpty())
        {
            return 0;
        }
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);
        var sql = $@"
UPDATE {_tableName}
SET {propertyValues.Keys.Select(p => $"{GetColumnName(p)}=@set_{p}").StringJoin($",{Environment.NewLine}")}
{whereSql.SqlText}
";
        foreach (var propertyValue in propertyValues)
        {
            whereSql.Parameters.Add($"set_{propertyValue.Key}", propertyValue.Value);
        }
        return _dbConnection.Value.Execute(sql, whereSql.Parameters);
    }

    public virtual int Update(TEntity entity, params Expression<Func<TEntity, object?>>[] propertyExpressions)
    {
        if (propertyExpressions.IsNullOrEmpty())
        {
            return UpdateWithout(entity, Array.Empty<string>());
        }
        //
        var keyEntries = PrimaryKeyColumns.Value
            .ToDictionary(p => p.Key, p => new KeyEntry()
            {
                PropertyName = p.Key,
                ColumnName = p.Value,
                Value = Guard.NotNull(typeof(TEntity).GetProperty(p.Key)).GetValue(entity)
            });
        if (keyEntries.Count == 0)
        {
            return -1;
        }
        //...
        var updateCols = propertyExpressions.Select(p => p.GetMemberName()).ToArray();
        var sql = $@"
UPDATE {_tableName}
SET {updateCols.Select(p => $"{GetColumnName(p)} = @set_{p}").StringJoin(", ")}
WHERE {keyEntries.Select(k => $"{k.Value.ColumnName} = @key_{k.Key}")}
";
        var parameters = new Dictionary<string, object?>();
        foreach (var col in updateCols)
        {
            parameters[$"set_{col}"] = GetColumnName(col);
        }
        foreach (var entry in keyEntries)
        {
            parameters[$"key_{entry.Key}"] = entry.Value.Value;
        }
        return _dbConnection.Value.Execute(sql, parameters);
    }

    public virtual int UpdateWithout(TEntity entity, params Expression<Func<TEntity, object?>>[] propertyExpressions)
    {
        Guard.NotNull(propertyExpressions, nameof(propertyExpressions));

        var keyEntries = PrimaryKeyColumns.Value
            .ToDictionary(p => p.Key, p => new KeyEntry()
            {
                PropertyName = p.Key,
                ColumnName = p.Value,
                Value = Guard.NotNull(typeof(TEntity).GetProperty(p.Key)).GetValue(entity)
            });
        if (keyEntries.Count == 0)
        {
            return -1;
        }
        //...
        var updateWithoutCols = propertyExpressions.Select(p => p.GetMemberName()).ToArray();
        var updateCols = ColumnMappings.Keys
            .Where(c => !updateWithoutCols.Contains(c) && !keyEntries.ContainsKey(c))
            .ToArray();
        if (updateCols.Length == 0)
            return 0;

        var sql = $@"
UPDATE {_tableName}
SET {updateCols.Select(p => $"{GetColumnName(p)} = @set_{p}").StringJoin(", ")}
WHERE {keyEntries.Select(k => $"{k.Value.ColumnName} = @key_{k.Key}")}
";
        var parameters = new Dictionary<string, object?>();
        foreach (var col in updateCols)
        {
            parameters[$"set_{col}"] = GetColumnName(col);
        }
        foreach (var entry in keyEntries)
        {
            parameters[$"key_{entry.Key}"] = entry.Value.Value;
        }
        return _dbConnection.Value.Execute(sql, parameters);
    }

    public virtual int Update(TEntity entity, params string[] propertyNames)
    {
        if (propertyNames.IsNullOrEmpty())
        {
            return UpdateWithout(entity, Array.Empty<string>());
        }
        //
        var keyEntries = PrimaryKeyColumns.Value
            .ToDictionary(p => p.Key, p => new KeyEntry()
            {
                PropertyName = p.Key,
                ColumnName = p.Value,
                Value = Guard.NotNull(typeof(TEntity).GetProperty(p.Key)).GetValue(entity)
            });
        if (keyEntries.Count == 0)
        {
            return -1;
        }
        //...
        var updateCols = propertyNames;
        var sql = $@"
UPDATE {_tableName}
SET {updateCols.Select(p => $"{GetColumnName(p)} = @set_{p}").StringJoin(", ")}
WHERE {keyEntries.Select(k => $"{k.Value.ColumnName} = @key_{k.Key}")}
";
        var parameters = new Dictionary<string, object?>();
        foreach (var col in updateCols)
        {
            parameters[$"set_{col}"] = GetColumnName(col);
        }
        foreach (var entry in keyEntries)
        {
            parameters[$"key_{entry.Key}"] = entry.Value.Value;
        }
        return _dbConnection.Value.Execute(sql, parameters);
    }

    public virtual int UpdateWithout(TEntity entity, params string[]? propertyNames)
    {
        var keyEntries = PrimaryKeyColumns.Value
            .ToDictionary(p => p.Key, p => new KeyEntry()
            {
                PropertyName = p.Key,
                ColumnName = p.Value,
                Value = Guard.NotNull(typeof(TEntity).GetProperty(p.Key)).GetValue(entity)
            });
        if (keyEntries.Count == 0)
        {
            return -1;
        }
        //...
        var updateWithoutCols = propertyNames ?? Array.Empty<string>();
        var updateCols = ColumnMappings.Keys
            .Where(c => !updateWithoutCols.Contains(c) && !keyEntries.ContainsKey(c))
            .ToArray();
        if (updateCols.Length == 0)
            return 0;

        var sql = $@"
UPDATE {_tableName}
SET {updateCols.Select(p => $"{GetColumnName(p)} = @set_{p}").StringJoin(", ")}
WHERE {keyEntries.Select(k => $"{k.Value.ColumnName} = @key_{k.Key}")}
";
        var parameters = new Dictionary<string, object?>();
        foreach (var col in updateCols)
        {
            parameters[$"set_{col}"] = GetColumnName(col);
        }
        foreach (var entry in keyEntries)
        {
            parameters[$"key_{entry.Key}"] = entry.Value.Value;
        }
        return _dbConnection.Value.Execute(sql, parameters);
    }

    public virtual Task<int> UpdateWithoutAsync(TEntity entity, string[] propertyNames, CancellationToken cancellationToken = default)
    {
        var keyEntries = PrimaryKeyColumns.Value
            .ToDictionary(p => p.Key, p => new KeyEntry()
            {
                PropertyName = p.Key,
                ColumnName = p.Value,
                Value = Guard.NotNull(typeof(TEntity).GetProperty(p.Key)).GetValue(entity)
            });
        if (keyEntries.Count == 0)
        {
            return Task.FromResult(-1);
        }

        //...
        var updateWithoutCols = propertyNames.Distinct().ToArray();
        var updateCols = ColumnMappings.Keys
            .Where(c => !updateWithoutCols.Contains(c) && !keyEntries.ContainsKey(c))
            .ToArray();
        if (updateCols.Length == 0)
            return Task.FromResult(0);

        var sql = $@"
UPDATE {_tableName}
SET {updateCols.Select(p => $"{GetColumnName(p)} = @set_{p}").StringJoin(", ")}
WHERE {keyEntries.Select(k => $"{k.Value.ColumnName} = @key_{k.Key}")}
";
        var parameters = new Dictionary<string, object?>();
        foreach (var col in updateCols)
        {
            parameters[$"set_{col}"] = GetColumnName(col);
        }
        foreach (var entry in keyEntries)
        {
            parameters[$"key_{entry.Key}"] = entry.Value.Value;
        }
        return _dbConnection.Value.ExecuteAsync(sql, paramInfo: parameters, cancellationToken: cancellationToken);
    }

    public virtual Task<int> UpdateAsync(TEntity entity, Expression<Func<TEntity, object?>>[] propertyExpressions, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(propertyExpressions, nameof(propertyExpressions));

        if (propertyExpressions.Length == 0)
        {
            return UpdateWithoutAsync(entity, Array.Empty<string>(), cancellationToken);
        }
        //
        var keyEntries = PrimaryKeyColumns.Value
            .ToDictionary(p => p.Key, p => new KeyEntry()
            {
                PropertyName = p.Key,
                ColumnName = p.Value,
                Value = Guard.NotNull(typeof(TEntity).GetProperty(p.Key)).GetValue(entity)
            });
        if (keyEntries.Count == 0)
        {
            return Task.FromResult(-1);
        }
        //...
        var updateCols = propertyExpressions.Select(p => p.GetMemberName()).ToArray();
        var sql = $@"
UPDATE {_tableName}
SET {updateCols.Select(p => $"{GetColumnName(p)} = @set_{p}").StringJoin(", ")}
WHERE {keyEntries.Select(k => $"{k.Value.ColumnName} = @key_{k.Key}")}
";
        var parameters = new Dictionary<string, object?>();
        foreach (var col in updateCols)
        {
            parameters[$"set_{col}"] = GetColumnName(col);
        }
        foreach (var entry in keyEntries)
        {
            parameters[$"key_{entry.Key}"] = entry.Value.Value;
        }
        return _dbConnection.Value.ExecuteAsync(sql, parameters, cancellationToken: cancellationToken);
    }

    public virtual Task<int> UpdateWithoutAsync(TEntity entity, Expression<Func<TEntity, object?>>[] propertyExpressions,
        CancellationToken cancellationToken = default)
    {
        var keyEntries = PrimaryKeyColumns.Value
          .ToDictionary(p => p.Key, p => new KeyEntry()
          {
              PropertyName = p.Key,
              ColumnName = p.Value,
              Value = Guard.NotNull(typeof(TEntity).GetProperty(p.Key)).GetValue(entity)
          });
        if (keyEntries.Count == 0)
        {
            return Task.FromResult(-1);
        }
        //...
        var updateWithoutCols = propertyExpressions.Select(x => x.GetMemberName()).Distinct().ToArray();
        var updateCols = ColumnMappings.Keys
            .Where(c => !updateWithoutCols.Contains(c) && !keyEntries.ContainsKey(c))
            .ToArray();
        if (updateCols.Length == 0)
            return Task.FromResult(0);

        var sql = $@"
UPDATE {_tableName}
SET {updateCols.Select(p => $"{GetColumnName(p)} = @set_{p}").StringJoin(", ")}
WHERE {keyEntries.Select(k => $"{k.Value.ColumnName} = @key_{k.Key}")}
";
        var parameters = new Dictionary<string, object?>();
        foreach (var col in updateCols)
        {
            parameters[$"set_{col}"] = GetColumnName(col);
        }
        foreach (var entry in keyEntries)
        {
            parameters[$"key_{entry.Key}"] = entry.Value.Value;
        }
        return _dbConnection.Value.ExecuteAsync(sql, paramInfo: parameters, cancellationToken: cancellationToken);
    }

    public virtual Task<int> UpdateAsync(TEntity entity, string[] propertyNames, CancellationToken cancellationToken = default)
    {
        Guard.NotNull(propertyNames, nameof(propertyNames));

        if (propertyNames.Length == 0)
        {
            return UpdateWithoutAsync(entity, Array.Empty<string>(), cancellationToken);
        }
        //
        var keyEntries = PrimaryKeyColumns.Value
            .ToDictionary(p => p.Key, p => new KeyEntry()
            {
                PropertyName = p.Key,
                ColumnName = p.Value,
                Value = Guard.NotNull(typeof(TEntity).GetProperty(p.Key)).GetValue(entity)
            });
        if (keyEntries.Count == 0)
        {
            return Task.FromResult(-1);
        }
        //...
        var updateCols = propertyNames;
        var sql = $@"
UPDATE {_tableName}
SET {updateCols.Select(p => $"{GetColumnName(p)} = @set_{p}").StringJoin(", ")}
WHERE {keyEntries.Select(k => $"{k.Value.ColumnName} = @key_{k.Key}")}
";
        var parameters = new Dictionary<string, object?>();
        foreach (var col in updateCols)
        {
            parameters[$"set_{col}"] = GetColumnName(col);
        }
        foreach (var entry in keyEntries)
        {
            parameters[$"key_{entry.Key}"] = entry.Value.Value;
        }
        return _dbConnection.Value.ExecuteAsync(sql, parameters, cancellationToken: cancellationToken);
    }

    public virtual Task<int> UpdateAsync(Expression<Func<TEntity, bool>> whereExpression,
        IDictionary<string, object?>? propertyValues, CancellationToken cancellationToken = default)
    {
        if (propertyValues is null || propertyValues.Count == 0)
        {
            return Task.FromResult(0);
        }
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);
        var sql = $@"
UPDATE {_tableName}
SET {propertyValues.Keys.Select(p => $"{GetColumnName(p)}=@set_{p}").StringJoin($",{Environment.NewLine}")}
{whereSql.SqlText}
";
        foreach (var propertyValue in propertyValues)
        {
            whereSql.Parameters.Add($"set_{propertyValue.Key}", propertyValue.Value);
        }
        return _dbConnection.Value.ExecuteAsync(sql, whereSql.Parameters, cancellationToken: cancellationToken);
    }

    public virtual int Delete(Expression<Func<TEntity, bool>> whereExpression)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);
        var sql = $@"
DELETE FROM {_tableName}
{whereSql.SqlText}
";
        return _dbConnection.Value.Execute(sql, whereSql.Parameters);
    }

    public virtual int Delete(TEntity entity)
    {
        var keyEntries = PrimaryKeyColumns.Value
            .ToDictionary(p => p.Key, p => new KeyEntry()
            {
                PropertyName = p.Key,
                ColumnName = p.Value,
                Value = Guard.NotNull(typeof(TEntity).GetProperty(p.Key)).GetValue(entity)
            });
        if (keyEntries.Count == 0)
        {
            return -1;
        }

        var sql = $@"
DELETE FROM {_tableName}
WHERE {keyEntries.Select(x => $"{x.Value.ColumnName} = @{x.Key}").StringJoin(" AND ")}
";
        return _dbConnection.Value.Execute(sql,
            keyEntries.ToDictionary(
                    x => x.Key,
                    x => x.Value.Value));
    }

    public virtual Task<int> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default)
    {
        var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression, ColumnMappings);
        var sql = $@"
DELETE FROM {_tableName}
{whereSql.SqlText}
";
        return _dbConnection.Value.ExecuteAsync(sql, whereSql.Parameters, cancellationToken: cancellationToken);
    }

    public virtual async Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        var keyEntries = PrimaryKeyColumns.Value
            .ToDictionary(p => p.Key, p => new KeyEntry()
            {
                PropertyName = p.Key,
                ColumnName = p.Value,
                Value = Guard.NotNull(typeof(TEntity).GetProperty(p.Key)).GetValue(entity)
            });
        if (keyEntries.Count == 0)
        {
            return -1;
        }

        var sql = $@"
DELETE FROM {_tableName}
WHERE {keyEntries.Select(x => $"{x.Value.ColumnName} = @{x.Key}").StringJoin(" AND ")}
";
        return await _dbConnection.Value.ExecuteAsync(sql,
            keyEntries.ToDictionary(
                x => x.Key,
                x => x.Value.Value), cancellationToken: cancellationToken);
    }

    public virtual int Execute(string sqlStr, object? param = null)
    => _dbConnection.Value.Execute(sqlStr, paramInfo: param);

    public virtual Task<int> ExecuteAsync(string sqlStr, object? param = null, CancellationToken cancellationToken = default)
    => _dbConnection.Value.ExecuteAsync(sqlStr, paramInfo: param, cancellationToken: cancellationToken);

    public virtual TResult ExecuteScalar<TResult>(string sqlStr, object? param = null)

    => _dbConnection.Value.ExecuteScalarTo<TResult>(sqlStr, paramInfo: param);

    public virtual Task<TResult> ExecuteScalarAsync<TResult>(string sqlStr, object? param = null, CancellationToken cancellationToken = default)

    => _dbConnection.Value.ExecuteScalarToAsync<TResult>(sqlStr, paramInfo: param, cancellationToken: cancellationToken);

    private string GetColumnName(string propertyName)
    {
        return ColumnMappings.TryGetValue(propertyName, out var colName)
            ? colName : propertyName;
    }
}
