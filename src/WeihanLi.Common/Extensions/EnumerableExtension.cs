// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System;
using System.Collections;
using System.Diagnostics.CodeAnalysis;
using WeihanLi.Common;
using WeihanLi.Common.Helpers.Combinatorics;
using WeihanLi.Common.Models;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class EnumerableExtension
{
    public static void ForEach<T>(this IEnumerable<T> ts, Action<T> action)
    {
        foreach (var t in ts)
        {
            action(t);
        }
    }

    public static void ForEach<T>(this IEnumerable<T> ts, Action<T, int> action)
    {
        var i = 0;
        foreach (var t in ts)
        {
            action(t, i);
            i++;
        }
    }

    public static async Task ForEachAsync<T>(this IEnumerable<T> ts, Func<T, Task> action)
    {
        foreach (var t in ts)
        {
            await action(t);
        }
    }

    public static async Task ForEachAsync<T>(this IEnumerable<T> ts, Func<T, int, Task> action)
    {
        var i = 0;
        foreach (var t in ts)
        {
            await action(t, i);
            i++;
        }
    }

    /// <summary>
    ///     A T[] extension method that converts an array to a read only.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <returns>A list of.</returns>
    public static IReadOnlyCollection<T> AsReadOnly<T>(this IEnumerable<T> @this)
    {
        return Array.AsReadOnly(@this.ToArray());
    }

    /// <summary>
    ///     An IEnumerable&lt;T&gt; extension method that queries if a not null or is empty.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="source">The collection to act on.</param>
    /// <returns>true if a not null or is t>, false if not.</returns>
    public static bool HasValue<T>([NotNullWhen(true)] this IEnumerable<T>? source)
    {
        return source != null && source.Any();
    }

    /// <summary>
    ///     Concatenates all the elements of a IEnumerable, using the specified separator between each element.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">An IEnumerable that contains the elements to concatenate.</param>
    /// <param name="separator">
    ///     The string to use as a separator. separator is included in the returned string only if
    ///     value has more than one element.
    /// </param>
    /// <returns>
    ///     A string that consists of the elements in value delimited by the separator string. If value is an empty array,
    ///     the method returns String.Empty.
    /// </returns>
    public static string StringJoin<T>(this IEnumerable<T> @this, string separator)
    {
        return string.Join(separator, @this);
    }

#if NETSTANDARD2_0

    public static IEnumerable<TSource> Prepend<TSource>(this IEnumerable<TSource> source, TSource value)
    {
        yield return value;

        foreach (var element in source)
        {
            yield return element;
        }
    }

    public static IEnumerable<TSource> Append<TSource>(this IEnumerable<TSource> source, TSource value)
    {
        foreach (var element in source)
        {
            yield return element;
        }

        yield return value;
    }

#endif

    #region Split

    /// <summary>
    /// 将一维集合分割成二维集合
    /// </summary>
    /// <param name="source">source</param>
    /// <param name="batchSize">每个一维集合的数量</param>
    public static IEnumerable<T[]> Split<T>(this IEnumerable<T> source, int batchSize)
    {
        using var enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
        {
            yield return Split(enumerator, batchSize).ToArray();
        }
    }

    private static IEnumerable<T> Split<T>(IEnumerator<T> enumerator, int batchSize)
    {
        do
        {
            yield return enumerator.Current;
        } while (--batchSize > 0 && enumerator.MoveNext());
    }

    #endregion Split

    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, Func<T, bool> predict, bool condition)
        => condition ? Guard.NotNull(source, nameof(source)).Where(predict) : source;

    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, Func<T, bool> predict, Func<bool> condition)
        => condition() ? Guard.NotNull(source, nameof(source)).Where(predict) : source;

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
        => Guard.NotNull(source, nameof(source)).Where(_ => _ != null)!;

    public static IEnumerable<T> Distinct<T>(this IEnumerable<T> source, Func<T?, T?, bool> comparer) where T : class
        => source.Distinct(new DynamicEqualityComparer<T>(comparer));

    // https://github.com/aspnet/EntityFrameworkCore/blob/release/3.0/src/EFCore.SqlServer/Utilities/EnumerableExtensions.cs
    private sealed class DynamicEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        private readonly Func<T?, T?, bool> _func;

        public DynamicEqualityComparer(Func<T?, T?, bool> func)
        {
            _func = func;
        }

        public bool Equals(T? x, T? y) => _func(x, y);

        public int GetHashCode(T obj) => 0; // force Equals
    }

    #region Linq

    /// <summary>
    /// LeftJoin extension
    /// </summary>
    /// <typeparam name="TOuter">outer</typeparam>
    /// <typeparam name="TInner">inner</typeparam>
    /// <typeparam name="TKey">TKey</typeparam>
    /// <typeparam name="TResult">TResult</typeparam>
    /// <param name="outer">outer collection</param>
    /// <param name="inner">inner collection</param>
    /// <param name="outerKeySelector">outerKeySelector</param>
    /// <param name="innerKeySelector">innerKeySelector</param>
    /// <param name="resultSelector">resultSelector</param>
    /// <returns></returns>
    public static IEnumerable<TResult> LeftJoin<TOuter, TInner, TKey, TResult>(this IEnumerable<TOuter> outer,
        IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector,
        Func<TOuter, TInner?, TResult> resultSelector)
    {
        return outer
            .GroupJoin(inner, outerKeySelector, innerKeySelector,
                (outerObj, inners) => new { outerObj, inners = inners.DefaultIfEmpty() })
            .SelectMany(a => a.inners.Select(innerObj => resultSelector(a.outerObj, innerObj)));
    }

    #endregion Linq

    #region ToPagedList

    /// <summary>
    /// ToPagedList
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="data">data</param>
    /// <param name="totalCount">totalCount</param>
    /// <returns></returns>
    public static IListResultWithTotal<T> ToListResultWithTotal<T>(this IEnumerable<T> data, int totalCount)
        => new ListResultWithTotal<T>
        {
            TotalCount = totalCount,
            Data = data is IReadOnlyList<T> dataList ? dataList : data.ToArray()
        };

    /// <summary>
    /// ToPagedList
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="data">data</param>
    /// <param name="pageNumber">pageNumber</param>
    /// <param name="pageSize">pageSize</param>
    /// <param name="totalCount">totalCount</param>
    /// <returns></returns>
    public static IPagedListResult<T> ToPagedList<T>(this IEnumerable<T> data, int pageNumber, int pageSize,
        int totalCount)
        => new PagedListResult<T>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            Data = data is IReadOnlyList<T> dataList ? dataList : data.ToArray()
        };

    /// <summary>
    /// ToPagedList
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    /// <param name="data">data</param>
    /// <param name="pageNumber">pageNumber</param>
    /// <param name="pageSize">pageSize</param>
    /// <param name="totalCount">totalCount</param>
    /// <returns></returns>
    public static IPagedListResult<T> ToPagedList<T>(this IReadOnlyList<T> data, int pageNumber, int pageSize,
        int totalCount)
        => new PagedListResult<T>
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = totalCount,
            Data = data
        };

    #endregion ToPagedList

    public static IEnumerable<IReadOnlyList<T>> GetCombinations<T>(this IEnumerable<T> values, int count,
        bool withRepetition = false)
    {
        return new Combinations<T>(values, count,
            withRepetition ? GenerateOption.WithRepetition : GenerateOption.WithoutRepetition);
    }

    public static IEnumerable<IReadOnlyList<T>> GetPermutations<T>(this IEnumerable<T> values,
        bool withRepetition = false, IComparer<T>? comparer = null)
    {
        return new Permutations<T>(values,
            withRepetition ? GenerateOption.WithRepetition : GenerateOption.WithoutRepetition, comparer);
    }

    public static IEnumerable<IGrouping<TKey, T>> GroupByEquality<T, TKey>(this IEnumerable<T> source,
        Func<T, TKey> keySelector,
        IEqualityComparer<TKey> keyComparer,
        Action<TKey, T>? keyAction = null, Action<T, TKey>? itemAction = null) where TKey : notnull
    {
        return GroupByEquality(source, keySelector, keyComparer.Equals, keyAction, itemAction);
    }

    public static IEnumerable<IGrouping<TKey, T>> GroupByEquality<T, TKey>(this IEnumerable<T> source,
        Func<T, TKey> keySelector,
        Func<TKey, TKey, bool> comparer,
        Action<TKey, T>? keyAction = null, Action<T, TKey>? itemAction = null)
    {
        var groups = new List<Grouping<TKey, T>>();
        foreach (var item in source)
        {
            var key = keySelector(item);
            var group = groups.FirstOrDefault(x => comparer(x.Key, key));
            if (group is null)
            {
                group = new Grouping<TKey, T>(key)
                {
                    item
                };
                groups.Add(group);
            }
            else
            {
                keyAction?.Invoke(group.Key, item);
                group.Add(item);
            }
        }

        if (itemAction != null)
        {
            foreach (var group in groups.Where(g => g.Count > 1))
            {
                foreach (var item in group)
                    itemAction.Invoke(item, group.Key);
            }
        }

        return groups;
    }

    private sealed class Grouping<TKey, T> : IGrouping<TKey, T>
    {
        private readonly List<T> _items = new();
        public Grouping(TKey key) => Key = key ?? throw new ArgumentNullException(nameof(key));

        public TKey Key { get; }

        public void Add(T t) => _items.Add(t);

        public int Count => _items.Count;

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
