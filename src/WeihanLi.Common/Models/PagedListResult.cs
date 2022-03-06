﻿// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

public interface IPagedListResult<out T> : IListResultWithTotal<T>
{
    int Count { get; }

    /// <summary>
    /// PageNumber
    /// </summary>
    int PageNumber { get; }

    /// <summary>
    /// PageSize
    /// </summary>
    int PageSize { get; }

    /// <summary>
    /// PageCount
    /// </summary>
    int PageCount { get; }
}

public interface IListResultWithTotal<out T>
{
    IReadOnlyList<T> Data { get; }

    int TotalCount { get; }
}

public static class EnumerableExtensions
{
    public static IEnumerator<T> GetEnumerator<T>(this IListResultWithTotal<T> listResult)
        => listResult.Data.GetEnumerator();
}

public class ListResultWithTotal<T> : IListResultWithTotal<T>
{
    public static readonly ListResultWithTotal<T> Empty = new();

    private IReadOnlyList<T> _data = Array.Empty<T>();

    public IReadOnlyList<T> Data
    {
        get => _data;
        set => _data = Guard.NotNull(value, nameof(value));
    }

    public int TotalCount { get; set; }
}

/// <summary>
/// 分页Model
/// </summary>
/// <typeparam name="T">Type</typeparam>
[Serializable]
public class PagedListResult<T> : IPagedListResult<T>
{
    public static readonly PagedListResult<T> Empty = new();

    private IReadOnlyList<T> _data = Array.Empty<T>();

    public IReadOnlyList<T> Data
    {
        get => _data;
        set => _data = Guard.NotNull(value, nameof(value));
    }

    private int _pageNumber = 1;

    public int PageNumber
    {
        get => _pageNumber;
        set
        {
            if (value > 0)
            {
                _pageNumber = value;
            }
        }
    }

    private int _pageSize = 10;

    public int PageSize
    {
        get => _pageSize;
        set
        {
            if (value > 0)
            {
                _pageSize = value;
            }
        }
    }

    private int _totalCount;

    public int TotalCount
    {
        get => _totalCount;
        set
        {
            if (value > 0)
            {
                _totalCount = value;
            }
        }
    }

    public int PageCount => (_totalCount + _pageSize - 1) / _pageSize;

    public T this[int index] => Data[index];

    public int Count => Data.Count;
}
