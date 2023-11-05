﻿using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using WeihanLi.Common;
using WeihanLi.Common.Models;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class QueryableExtension
{
    public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, bool condition)
    {
        return condition ? Guard.NotNull(source, nameof(source)).Where(predicate) : source;
    }

    public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, Func<bool> conditionFunc)
    {
        return conditionFunc() ? Guard.NotNull(source, nameof(source)).Where(predicate) : source;
    }

    /// <summary>
    /// Converts the specified source to <see cref="IPagedListResult{T}"/> by the specified <paramref name="pageNumber"/> and <paramref name="pageSize"/>.
    /// </summary>
    /// <typeparam name="T">The type of the source.</typeparam>
    /// <param name="source">The source to paging.</param>
    /// <param name="pageNumber">The number of the page, start from 1.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <returns>An instance of  implements <see cref="IPagedListResult{T}"/> interface.</returns>
    public static ListResultWithTotal<T> ToListResultWithTotal<T>(this IQueryable<T> source, int pageNumber, int pageSize)
    {
        if (pageNumber <= 0)
        {
            pageNumber = 1;
        }
        if (pageSize <= 0)
        {
            pageSize = 10;
        }
        var count = source.Count();
        if (count == 0)
        {
            return ListResultWithTotal<T>.Empty;
        }

        if (pageNumber > 1)
        {
            source = source.Skip((pageNumber - 1) * pageSize);
        }
        var items = source.Take(pageSize).ToArray();

        return new ListResultWithTotal<T>()
        {
            TotalCount = count,
            Data = items
        };
    }

    /// <summary>
    /// Converts the specified source to <see cref="IPagedListResult{T}"/> by the specified <paramref name="pageNumber"/> and <paramref name="pageSize"/>.
    /// </summary>
    /// <typeparam name="T">The type of the source.</typeparam>
    /// <param name="source">The source to paging.</param>
    /// <param name="pageNumber">The number of the page, start from 1.</param>
    /// <param name="pageSize">The size of the page.</param>
    /// <returns>An instance of  implements <see cref="IPagedListResult{T}"/> interface.</returns>
    public static PagedListResult<T> ToPagedList<T>(this IQueryable<T> source, int pageNumber, int pageSize)
    {
        if (pageNumber <= 0)
        {
            pageNumber = 1;
        }
        if (pageSize <= 0)
        {
            pageSize = 10;
        }
        var count = source.Count();
        if (count == 0)
        {
            return PagedListResult<T>.Empty;
        }

        if (pageNumber > 1)
        {
            source = source.Skip((pageNumber - 1) * pageSize);
        }
        var items = source.Take(pageSize).ToArray();

        return new PagedListResult<T>()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = count,
            Data = items
        };
    }

    /// <summary>
    /// OrderBy propertyName
    /// </summary>
    /// <typeparam name="T">T</typeparam>
    /// <param name="source">source</param>
    /// <param name="propertyName">propertyName to orderBy</param>
    /// <param name="isAsc">is ascending</param>
    /// <returns></returns>
    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string propertyName, bool isAsc = false)
    {
        Guard.NotNull(source);
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException(nameof(propertyName));
        }

        var type = typeof(T);
        var arg = Expression.Parameter(type, "x");
        var propertyInfo = type.GetProperty(propertyName);
        if (null == propertyInfo)
        {
            throw new InvalidOperationException($"{propertyName} does not exists");
        }

        Expression expression = Expression.Property(arg, propertyInfo);
        type = propertyInfo.PropertyType;

        var delegateType = typeof(Func<,>).MakeGenericType(type, type);
        var lambda = Expression.Lambda(delegateType, expression, arg);

        var methodName = isAsc ? "OrderBy" : "OrderByDescending";
        var result = typeof(Queryable).GetMethods().Single(
                method => method.Name == methodName
                          && method.IsGenericMethodDefinition
                          && method.GetGenericArguments().Length == 2
                          && method.GetParameters().Length == 2)
            .MakeGenericMethod(type, type)
            .Invoke(null, new object[] { source, lambda });
        return (IQueryable<T>)Guard.NotNull(result);
    }
}
