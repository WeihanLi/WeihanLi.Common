using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using WeihanLi.Common.Models;

namespace WeihanLi.Common.Data
{
    /// <summary>
    /// Represents an entity repository
    /// </summary>
    /// <typeparam name="TEntity">Entity type</typeparam>
    public interface IRepository<TEntity>
    {
        /// <summary>
        /// Count
        /// </summary>
        int Count(Expression<Func<TEntity, bool>> whereExpression);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default);

        /// <summary>
        /// LongCount
        /// </summary>
        long LongCount(Expression<Func<TEntity, bool>> whereExpression);

        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default);

        /// <summary>
        /// Exist
        /// </summary>
        bool Exist(Expression<Func<TEntity, bool>> whereExpression);

        Task<bool> ExistAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get top 1 entity
        /// </summary>
        TEntity Fetch(Expression<Func<TEntity, bool>> whereExpression);

        Task<TEntity> FetchAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default);

        TEntity Fetch<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false);

        Task<TEntity> FetchAsync<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get List
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <returns></returns>
        List<TEntity> Select(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// Get Entity List
        /// </summary>
        /// <param name="whereExpression">where Expression</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns></returns>
        Task<List<TEntity>> SelectAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default);

        /// <summary>
        /// Get List, Top n
        /// </summary>
        /// <param name="count">count</param>
        /// <param name="whereExpression">whereExpression</param>
        /// <param name="orderByExpression">orderBy</param>
        /// <param name="ascending">is ascending</param>
        /// <returns></returns>
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
        Task<IPagedListResult<TEntity>> PagedAsync<TProperty>(int pageNumber, int pageSize, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// Insert a entity
        /// </summary>
        /// <param name="entity">Entity</param>
        int Insert(TEntity entity);

        /// <summary>
        /// Insert a entity
        /// </summary>
        /// <param name="entity">Entity</param>
        /// <param name="cancellationToken">cancellationToken</param>
        Task<int> InsertAsync(TEntity entity, CancellationToken cancellationToken = default);

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        int Insert(IEnumerable<TEntity> entities);

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>rows affected</returns>
        Task<int> InsertAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

        /// <summary>
        /// update entity specific property by where
        /// </summary>
        /// <typeparam name="TProperty">TProperty</typeparam>
        /// <param name="whereExpression">where</param>
        /// <param name="propertyExpression">property</param>
        /// <param name="value">new property value</param>
        /// <returns></returns>
        int Update<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> propertyExpression, object value);

        /// <summary>
        /// update entity specific property by where
        /// </summary>
        /// <typeparam name="TProperty">TProperty</typeparam>
        /// <param name="whereExpression">where</param>
        /// <param name="propertyExpression">property</param>
        /// <param name="value">new property value</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns></returns>
        Task<int> UpdateAsync<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> propertyExpression, object value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Update entities properties by where
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <param name="propertyValues">propertyValues to update</param>
        /// <returns>updated rows count</returns>
        int Update(Expression<Func<TEntity, bool>> whereExpression, IDictionary<string, object> propertyValues);

        /// <summary>
        /// Update entities properties by where
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <param name="propertyValues">propertyValues to update</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>updated rows count</returns>
        Task<int> UpdateAsync(Expression<Func<TEntity, bool>> whereExpression, IDictionary<string, object> propertyValues, CancellationToken cancellationToken = default);

        /// <summary>
        /// Delete entities by where
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        int Delete(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// Delete entities by  where
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <param name="cancellationToken">cancellationToken</param>
        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression, CancellationToken cancellationToken = default);
    }
}
