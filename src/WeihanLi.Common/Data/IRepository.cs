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
        #region Methods

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
        IEnumerable<TEntity> Select(Expression<Func<TEntity, bool>> whereExpression);

        Task<IEnumerable<TEntity>> SelectAsync(Expression<Func<TEntity, bool>> whereExpression);

        /// <summary>
        /// Get Paged List
        /// </summary>
        PagedListModel<TEntity> Paged<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, int pageIndex, int pageSize);

        Task<PagedListModel<TEntity>> PagedAsync<TProperty>(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, int pageIndex, int pageSize);

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

        #endregion Methods
    }
}
