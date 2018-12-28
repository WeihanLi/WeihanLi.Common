using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WeihanLi.Common.Models;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Data
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : new()
    {
        private static readonly Type EntityType = typeof(TEntity);

        private readonly string _tableName;
        private readonly DbConnection _dbConnection;

        public Repository(DbConnection dbConnection)
        {
            _dbConnection = dbConnection;

            _tableName = EntityType.IsDefined(typeof(TableAttribute))
                ? EntityType.GetCustomAttribute<TableAttribute>().Name
                : EntityType.Name;
        }

        public TEntity Fetch(Expression<Func<TEntity, bool>> whereExpression)
        {
            var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression);
            var sql = $@"
SELECT TOP 1 * FROM {_tableName}
{whereSql.SqlText}
";
            return _dbConnection.Fetch<TEntity>(sql, whereSql.Parameters);
        }

        public Task<TEntity> FetchAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression);
            var sql = $@"
SELECT TOP 1 * FROM {_tableName}
{whereSql.SqlText}
";
            return _dbConnection.FetchAsync<TEntity>(sql, whereSql.Parameters);
        }

        public IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> whereExpression)
        {
            var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression);
            var sql = $@"
SELECT * FROM {_tableName}
{whereSql.SqlText}
";
            return _dbConnection.Select<TEntity>(sql, whereSql.Parameters);
        }

        public Task<IEnumerable<TEntity>> SelectAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression);
            var sql = $@"
SELECT * FROM {_tableName}
{whereSql.SqlText}
";
            return _dbConnection.SelectAsync<TEntity>(sql, whereSql.Parameters);
        }

        public PagedListModel<TEntity> Paged<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, int pageIndex, int pageSize)
        {
            var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression);

            var sql = $@"
SELECT COUNT(1) FROM {_tableName}
{whereSql.SqlText}
";
            var total = _dbConnection.ExecuteScalarTo<int>(sql, whereSql.Parameters);

            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            if (pageSize <= 0)
            {
                pageSize = 10;
            }

            var offset = (pageIndex - 1) * pageSize;

            sql = $@"
SELECT * FROM {_tableName}
{whereSql.SqlText}
ORDER BY {orderByExpression.GetMemberName()}
OFFSET {offset} ROWS
FETCH NEXT {pageSize} ROWS ONLY
";

            return _dbConnection.Select<TEntity>(sql, whereSql.Parameters).ToPagedListModel(pageIndex, pageSize, total);
        }

        public async Task<PagedListModel<TEntity>> PagedAsync<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, int pageIndex, int pageSize)
        {
            var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression);

            var sql = $@"
SELECT COUNT(1) FROM {_tableName}
{whereSql.SqlText}
";
            var total = await _dbConnection.ExecuteScalarToAsync<int>(sql, whereSql.Parameters);

            if (pageIndex <= 0)
            {
                pageIndex = 1;
            }
            if (pageSize <= 0)
            {
                pageSize = 10;
            }

            var offset = (pageIndex - 1) * pageSize;

            sql = $@"
SELECT * FROM {_tableName}
{whereSql.SqlText}
ORDER BY {orderByExpression.GetMemberName()}
OFFSET {offset} ROWS
FETCH NEXT {pageSize} ROWS ONLY
";

            return (await _dbConnection.SelectAsync<TEntity>(sql, whereSql.Parameters)).ToPagedListModel(pageIndex, pageSize, total);
        }

        public int Insert(TEntity entity)
        {
            var fields = CacheUtil.TypePropertyCache.GetOrAdd(EntityType, t => t.GetProperties())
                .Where(p => !p.IsDefined(typeof(DatabaseGeneratedAttribute)) && !p.IsDefined(typeof(NotMappedAttribute)))
                .Select(_ => _.Name)
                .ToArray();

            var paramDictionary = new Dictionary<string, object>();

            var sqlBuilder = new StringBuilder($@"INSERT INTO {_tableName} (");
            foreach (var field in fields)
            {
                sqlBuilder.AppendLine($"{field},");
            }

            sqlBuilder.Remove(sqlBuilder.Length - 2, 1);
            sqlBuilder.AppendLine(")");
            sqlBuilder.AppendLine("VALUES (");

            foreach (var field in fields)
            {
                sqlBuilder.AppendLine($"@{field},");
                paramDictionary.Add($"{field}", entity.GetPropertyValue(field));
            }

            sqlBuilder.Remove(sqlBuilder.Length - 2, 1);
            sqlBuilder.AppendLine(")");

            return _dbConnection.Execute(sqlBuilder.ToString(), paramDictionary);
        }

        public Task<int> InsertAsync(TEntity entity)
        {
            var fields = CacheUtil.TypePropertyCache.GetOrAdd(EntityType, t => t.GetProperties())
                .Where(p => !p.IsDefined(typeof(DatabaseGeneratedAttribute)) && !p.IsDefined(typeof(NotMappedAttribute)))
                .Select(_ => _.Name)
                .ToArray();
            var paramDictionary = new Dictionary<string, object>();

            var sqlBuilder = new StringBuilder($@"INSERT INTO {_tableName} (");
            foreach (var field in fields)
            {
                sqlBuilder.AppendLine($"{field},");
            }

            sqlBuilder.Remove(sqlBuilder.Length - 2, 1);
            sqlBuilder.AppendLine(")");
            sqlBuilder.AppendLine("VALUES (");

            foreach (var field in fields)
            {
                sqlBuilder.AppendLine($"@{field},");
                paramDictionary.Add($"{field}", entity.GetPropertyValue(field));
            }

            sqlBuilder.Remove(sqlBuilder.Length - 2, 1);
            sqlBuilder.AppendLine(")");

            return _dbConnection.ExecuteAsync(sqlBuilder.ToString(), paramDictionary);
        }

        public int Insert(IEnumerable<TEntity> entities)
        {
            var count = entities?.Count() ?? 0;
            if (count == 0)
            {
                return 0;
            }
            if (count > 1000)
            {
                return -1; // too large, not supported
            }
            var fields = CacheUtil.TypePropertyCache.GetOrAdd(EntityType, t => t.GetProperties())
                .Where(p => !p.IsDefined(typeof(DatabaseGeneratedAttribute)) && !p.IsDefined(typeof(NotMappedAttribute)))
                .Select(_ => _.Name)
                .ToArray();
            var paramDictionary = new Dictionary<string, object>();
            var sqlBuilder = new StringBuilder($@"INSERT INTO {_tableName} (");
            foreach (var field in fields)
            {
                sqlBuilder.AppendLine($"{field},");
            }

            sqlBuilder.Remove(sqlBuilder.Length - 2, 1);
            sqlBuilder.AppendLine(")");
            sqlBuilder.AppendLine("VALUES");

            for (var i = 0; i < count; i++)
            {
                sqlBuilder.AppendLine("(");
                foreach (var field in fields)
                {
                    sqlBuilder.AppendLine($"@{field}_{i},");
                    paramDictionary.Add($"{field}_{i}", EntityType.GetPropertyValue(field));
                }
                sqlBuilder.Remove(sqlBuilder.Length - 2, 1);
                sqlBuilder.AppendLine("),");
            }
            sqlBuilder.Remove(sqlBuilder.Length - 2, 1);

            return _dbConnection.Execute(sqlBuilder.ToString(), paramDictionary);
        }

        public Task<int> InsertAsync(IEnumerable<TEntity> entities)
        {
            var count = entities?.Count() ?? 0;
            if (count == 0)
            {
                return Task.FromResult(0);
            }
            if (count > 1000)
            {
                return Task.FromResult(-1); // too large, not supported
            }
            var fields = CacheUtil.TypePropertyCache.GetOrAdd(EntityType, t => t.GetProperties())
                .Where(p => !p.IsDefined(typeof(DatabaseGeneratedAttribute)) && !p.IsDefined(typeof(NotMappedAttribute)))
                .Select(_ => _.Name)
                .ToArray();
            var paramDictionary = new Dictionary<string, object>();
            var sqlBuilder = new StringBuilder($@"INSERT INTO {_tableName} (");
            foreach (var field in fields)
            {
                sqlBuilder.AppendLine($"{field},");
            }

            sqlBuilder.Remove(sqlBuilder.Length - 2, 1);
            sqlBuilder.AppendLine(")");
            sqlBuilder.AppendLine("VALUES");

            for (var i = 0; i < count; i++)
            {
                sqlBuilder.AppendLine("(");
                foreach (var field in fields)
                {
                    sqlBuilder.AppendLine($"@{field}_{i},");
                    paramDictionary.Add($"{field}_{i}", EntityType.GetPropertyValue(field));
                }
                sqlBuilder.Remove(sqlBuilder.Length - 2, 1);
                sqlBuilder.AppendLine("),");
            }
            sqlBuilder.Remove(sqlBuilder.Length - 2, 1);

            return _dbConnection.ExecuteAsync(sqlBuilder.ToString(), paramDictionary);
        }

        public int Update<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> propertyExpression, object value)
        {
            var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression);
            var propertyName = propertyExpression.GetMemberName();
            var sql = $@"
UPDATE {_tableName}
SET {propertyName} = @set_{propertyName}
{whereSql.SqlText}
";
            whereSql.Parameters.Add($"set_{propertyName}", value);
            return _dbConnection.Execute(sql, whereSql.Parameters);
        }

        public Task<int> UpdateAsync<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> propertyExpression, object value)
        {
            var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression);
            var propertyName = propertyExpression.GetMemberName();
            var sql = $@"
UPDATE {_tableName}
SET {propertyName} = @set_{propertyName}
{whereSql.SqlText}
";
            whereSql.Parameters.Add($"set_{propertyName}", value);
            return _dbConnection.ExecuteAsync(sql, whereSql.Parameters);
        }

        public int Update(Expression<Func<TEntity, bool>> whereExpression, IDictionary<string, object> propertyValues)
        {
            if (propertyValues == null || propertyValues.Count == 0)
            {
                return 0;
            }
            var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression);
            var sql = $@"
UPDATE {_tableName}
SET {propertyValues.Keys.Select(p => $"{p}=@set_{p}").StringJoin($",{Environment.NewLine}")}
{whereSql.SqlText}
";
            foreach (var propertyValue in propertyValues)
            {
                whereSql.Parameters.Add($"set_{propertyValue.Key}", propertyValue.Value);
            }
            return _dbConnection.Execute(sql, whereSql.Parameters);
        }

        public Task<int> UpdateAsync(Expression<Func<TEntity, bool>> whereExpression, IDictionary<string, object> propertyValues)
        {
            if (propertyValues == null || propertyValues.Count == 0)
            {
                return Task.FromResult(0);
            }
            var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression);
            var sql = $@"
UPDATE {_tableName}
SET {propertyValues.Keys.Select(p => $"{p}=@set_{p}").StringJoin($",{Environment.NewLine}")}
{whereSql.SqlText}
";
            foreach (var propertyValue in propertyValues)
            {
                whereSql.Parameters.Add($"set_{propertyValue.Key}", propertyValue.Value);
            }
            return _dbConnection.ExecuteAsync(sql, whereSql.Parameters);
        }

        public int Delete(Expression<Func<TEntity, bool>> whereExpression)
        {
            var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression);
            var sql = $@"
DELETE FROM {_tableName}
{whereSql.SqlText}
";
            return _dbConnection.Execute(sql, whereSql.Parameters);
        }

        public Task<int> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression)
        {
            var whereSql = SqlExpressionParser.ParseWhereExpression(whereExpression);
            var sql = $@"
DELETE FROM {_tableName}
{whereSql.SqlText}
";
            return _dbConnection.ExecuteAsync(sql, whereSql.Parameters);
        }
    }
}
