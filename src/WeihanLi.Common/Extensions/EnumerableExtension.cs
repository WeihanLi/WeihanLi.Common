// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common.Helpers.Combinatorics;
using WeihanLi.Common.Models;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class EnumerableExtension
{

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
            Data = data as IReadOnlyList<T> ?? data.ToArray()
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
}
