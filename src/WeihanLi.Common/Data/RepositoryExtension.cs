using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace WeihanLi.Common.Data
{
    public static class RepositoryExtension
    {
        public static int Count<TEntity>(this IRepository<TEntity> repository) => repository.Count(x => true);

        public static Task<int> CountAsync<TEntity>(this IRepository<TEntity> repository, CancellationToken cancellationToken = default) =>
            repository.CountAsync(x => true, cancellationToken);

        public static long LongCount<TEntity>(this IRepository<TEntity> repository) => repository.LongCount(x => true);

        public static Task<long> LongCountAsync<TEntity>(this IRepository<TEntity> repository, CancellationToken cancellationToken = default) =>
            repository.LongCountAsync(x => true, cancellationToken);

        public static TEntity Fetch<TEntity>(this IRepository<TEntity> repository) => repository.Fetch(x => true);

        public static Task<TEntity> FetchAsync<TEntity>(this IRepository<TEntity> repository, CancellationToken cancellationToken = default) =>
            repository.FetchAsync(x => true, cancellationToken);

        public static TEntity Fetch<TEntity, TProperty>(this IRepository<TEntity> repository, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false) => repository.Fetch(x => true, orderByExpression, ascending);

        public static Task<TEntity> FetchAsync<TEntity, TProperty>(this IRepository<TEntity> repository, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false, CancellationToken cancellationToken = default) =>
            repository.FetchAsync(x => true, orderByExpression, ascending, cancellationToken);

        public static List<TEntity> GetAll<TEntity>(this IRepository<TEntity> repository) =>
            repository.Select(x => true);

        public static Task<List<TEntity>> GetAllAsync<TEntity>(this IRepository<TEntity> repository, CancellationToken cancellationToken = default) =>
            repository.SelectAsync(x => true, cancellationToken);

        public static List<TEntity> Top<TEntity, TProperty>(this IRepository<TEntity> repository, int count, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false) =>
            repository.Select(count, x => true, orderByExpression, ascending);

        public static Task<List<TEntity>> TopAsync<TEntity, TProperty>(this IRepository<TEntity> repository, int count, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false, CancellationToken cancellationToken = default) =>
            repository.SelectAsync(count, x => true, orderByExpression, ascending, cancellationToken);

        public static List<TEntity> Top<TEntity, TProperty>(this IRepository<TEntity> repository, int count, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false) =>
            repository.Select(count, whereExpression, orderByExpression, ascending);

        public static Task<List<TEntity>> TopAsync<TEntity, TProperty>(this IRepository<TEntity> repository, int count, Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, TProperty>> orderByExpression, bool ascending = false, CancellationToken cancellationToken = default) =>
            repository.SelectAsync(count, whereExpression, orderByExpression, ascending, cancellationToken);
    }
}
