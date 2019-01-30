using System;
using System.Collections.Generic;
using System.Linq.Expressions;
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
        long Count(Expression<Func<TEntity, bool>> whereExpression);

        Task<long> CountAsync(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// Exist
        /// </summary>
        bool Exist(Expression<Func<TEntity, bool>> whereExpression);

        Task<bool> ExistAsync(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// Get top 1 entity
        /// </summary>
        TEntity Fetch(Expression<Func<TEntity, bool>> whereExpression);

        Task<TEntity> FetchAsync(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// Get List
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <returns></returns>
        List<TEntity> Select(Expression<Func<TEntity, bool>> whereExpression);

        Task<List<TEntity>> SelectAsync(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// Get List
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        /// <returns></returns>
        List<TEntity> Select<TProperty>(int count, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool isAsc = false);

        Task<List<TEntity>> SelectAsync<TProperty>(int count, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool isAsc = false);

        /// <summary>
        /// Get Paged List
        /// </summary>
        PagedListModel<TEntity> Paged<TProperty>(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool isAsc = false);

        Task<PagedListModel<TEntity>> PagedAsync<TProperty>(int pageIndex, int pageSize, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool isAsc = false);

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">Entity</param>
        int Insert(TEntity entity);

        Task<int> InsertAsync(TEntity entity);

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        int Insert(IEnumerable<TEntity> entities);

        Task<int> InsertAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Update entity
        /// </summary>
        int Update<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> propertyExpression, object value);

        Task<int> UpdateAsync<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> propertyExpression, object value);

        /// <summary>
        /// Update entities
        /// </summary>
        int Update(Expression<Func<TEntity, bool>> whereExpression, IDictionary<string, object> propertyValues);

        Task<int> UpdateAsync(Expression<Func<TEntity, bool>> whereExpression, IDictionary<string, object> propertyValues);

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="whereExpression">whereExpression</param>
        int Delete(Expression<Func<TEntity, bool>> whereExpression);

        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> whereExpression);
    }
}
