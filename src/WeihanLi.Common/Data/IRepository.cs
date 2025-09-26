using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using WeihanLi.Common.Models;

namespace WeihanLi.Common.Data;

public interface IReadOnlyRepository<TEntity>
{
    /// <summary>
    /// Count
    /// </summary>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    int Count(Expression<Func<TEntity, bool>> whereExpression);

    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    Task<int> CountAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default);

    /// <summary>
    /// LongCount
    /// </summary>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    long LongCount(Expression<Func<TEntity, bool>> whereExpression);

    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    Task<long> LongCountAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exist
    /// </summary>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    bool Exist(Expression<Func<TEntity, bool>> whereExpression);

    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    Task<bool> ExistAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get top 1 entity
    /// </summary>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    TEntity? Fetch(Expression<Func<TEntity, bool>> whereExpression);

    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    Task<TEntity?> FetchAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default);

    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    TEntity? Fetch<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false);

    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    Task<TEntity?> FetchAsync<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get List
    /// </summary>
    /// <param name="whereExpression">whereExpression</param>
    /// <returns></returns>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    List<TEntity> Select(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// Get Entity List
    /// </summary>
    /// <param name="whereExpression">where Expression</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns></returns>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    Task<List<TEntity>> SelectAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get List, Top n
    /// </summary>
    /// <param name="count">count</param>
    /// <param name="whereExpression">whereExpression</param>
    /// <param name="orderByExpression">orderBy</param>
    /// <param name="ascending">is ascending</param>
    /// <returns></returns>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    List<TEntity> Select<TProperty>(int count, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false);

    /// <summary>
    /// Get List, Top n
    /// </summary>
    /// <param name="count">count</param>
    /// <param name="whereExpression">whereExpression</param>
    /// <param name="orderByExpression">orderBy</param>
    /// <param name="ascending">is ascending</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns></returns>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    Task<List<TEntity>> SelectAsync<TProperty>(int count, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get Paged List
    /// </summary>
    /// <typeparam name="TProperty">property to orderBy</typeparam>
    /// <param name="pageNumber">pageNumber from 1</param>
    /// <param name="pageSize">pageSize</param>
    /// <param name="whereExpression">whereExpression</param>
    /// <param name="orderByExpression">orderByExpression</param>
    /// <param name="ascending">is ascending</param>
    /// <returns></returns>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    IPagedListResult<TEntity> Paged<TProperty>(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false);

    /// <summary>
    /// Get Paged List
    /// </summary>
    /// <typeparam name="TProperty">property to orderBy</typeparam>
    /// <param name="pageNumber">pageNumber from 1</param>
    /// <param name="pageSize">pageSize</param>
    /// <param name="whereExpression">whereExpression</param>
    /// <param name="orderByExpression">orderByExpression</param>
    /// <param name="ascending">is ascending</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns></returns>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    Task<IPagedListResult<TEntity>> PagedAsync<TProperty>(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false, CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents an entity repository
/// </summary>
/// <typeparam name="TEntity">Entity type</typeparam>
public interface IRepository<TEntity> : IReadOnlyRepository<TEntity>
{
    /// <summary>
    /// Insert a entity
    /// </summary>
    /// <param name="entity">Entity</param>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    int Insert(TEntity entity);

    /// <summary>
    /// Insert a entity
    /// </summary>
    /// <param name="entity">Entity</param>
    /// <param name="cancellationToken">cancellationToken</param>
    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    Task<int> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Insert entities
    /// </summary>
    /// <param name="entities">Entities</param>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    int Insert(IEnumerable<TEntity> entities);

    /// <summary>
    /// Insert entities
    /// </summary>
    /// <param name="entities">Entities</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>rows affected</returns>
    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    Task<int> InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>
    /// update entity specific property by where
    /// </summary>
    /// <typeparam name="TProperty">TProperty</typeparam>
    /// <param name="whereExpression">where</param>
    /// <param name="propertyExpression">property</param>
    /// <param name="value">new property value</param>
    /// <returns></returns>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    int Update<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> propertyExpression, object? value);

    /// <summary>
    /// update entity specific property by where
    /// </summary>
    /// <typeparam name="TProperty">TProperty</typeparam>
    /// <param name="whereExpression">where</param>
    /// <param name="propertyExpression">property</param>
    /// <param name="value">new property value</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns></returns>
    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    Task<int> UpdateAsync<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> propertyExpression, object? value, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update entities properties by where
    /// </summary>
    /// <param name="whereExpression">whereExpression</param>
    /// <param name="propertyValues">propertyValues to update</param>
    /// <returns>updated rows count</returns>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    int Update(Expression<Func<TEntity, bool>> whereExpression, IDictionary<string, object?> propertyValues);

    /// <summary>
    /// update entities with specific properties
    /// </summary>
    /// <param name="entity">entity</param>
    /// <param name="propertyExpressions">propertyExpressions</param>
    /// <returns>rows affected</returns>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    int Update(TEntity entity, params Expression<Func<TEntity, object?>>[] propertyExpressions);

    /// <summary>
    /// Update entity without specific properties
    /// </summary>
    /// <param name="entity">entity</param>
    /// <param name="propertyExpressions">properties not to update</param>
    /// <returns>affected rows</returns>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    int UpdateWithout(TEntity entity, params Expression<Func<TEntity, object?>>[] propertyExpressions);

    /// <summary>
    /// update entities with specific properties
    /// </summary>
    /// <param name="entity">entity</param>
    /// <param name="propertyNames">propertyNames</param>
    /// <returns>rows affected</returns>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    int Update(TEntity entity, params string[] propertyNames);

    /// <summary>
    /// Update entity without specific properties
    /// </summary>
    /// <param name="entity">entity</param>
    /// <param name="propertyNames">properties not to update</param>
    /// <returns>affected rows</returns>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    int UpdateWithout(TEntity entity, params string[] propertyNames);

    /// <summary>
    /// Update entity without specific properties
    /// </summary>
    /// <param name="entity">entity</param>
    /// <param name="propertyNames">properties not to update</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>affected rows</returns>
    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    Task<int> UpdateWithoutAsync(TEntity entity, string[] propertyNames, CancellationToken cancellationToken = default);

    /// <summary>
    /// update entities with specific properties
    /// </summary>
    /// <param name="entity">entity</param>
    /// <param name="propertyExpressions">propertyExpressions</param>
    /// <param name="cancellationToken"></param>
    /// <returns>rows affected</returns>
    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    Task<int> UpdateAsync(TEntity entity, Expression<Func<TEntity, object?>>[] propertyExpressions, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update entity without specific properties
    /// </summary>
    /// <param name="entity">entity</param>
    /// <param name="propertyExpressions">properties not to update</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>affected rows</returns>
    Task<int> UpdateWithoutAsync(TEntity entity, Expression<Func<TEntity, object?>>[] propertyExpressions, CancellationToken cancellationToken = default);

    /// <summary>
    /// update entities with specific properties
    /// </summary>
    /// <param name="entity">entity</param>
    /// <param name="propertyNames">propertyNames</param>
    /// <param name="cancellationToken"></param>
    /// <returns>rows affected</returns>
    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    Task<int> UpdateAsync(TEntity entity, string[] propertyNames, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update entities properties by where
    /// </summary>
    /// <param name="whereExpression">whereExpression</param>
    /// <param name="propertyValues">propertyValues to update</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>updated rows count</returns>
    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    Task<int> UpdateAsync(Expression<Func<TEntity, bool>> whereExpression, IDictionary<string, object?> propertyValues, CancellationToken cancellationToken = default);

    /// <summary>
    /// Delete entities by where
    /// </summary>
    /// <param name="whereExpression">whereExpression</param>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    int Delete(Expression<Func<TEntity, bool>> whereExpression);

    /// <summary>
    /// delete entity
    /// </summary>
    /// <param name="entity">entity</param>
    /// <returns>rows affected</returns>
    [RequiresDynamicCode("Database operations may require dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Database operations may use reflection which requires unreferenced code.")]
    int Delete(TEntity entity);

    /// <summary>
    /// Delete entities by  where
    /// </summary>
    /// <param name="whereExpression">whereExpression</param>
    /// <param name="cancellationToken">cancellationToken</param>
    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    Task<int> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default);

    /// <summary>
    /// delete entity async
    /// </summary>
    /// <param name="entity">entity</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>rows affected</returns>
    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    Task<int> DeleteAsync(TEntity entity, CancellationToken cancellationToken = default);
}
