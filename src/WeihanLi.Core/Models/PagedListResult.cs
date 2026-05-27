// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

/// <summary>
/// Represents a paged list result.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public interface IPagedListResult<out T> : IListResultWithTotal<T>
{
    /// <summary>
    /// Gets the item count in the current page.
    /// </summary>
    int Count { get; }

    /// <summary>
    /// Gets the one-based page number.
    /// </summary>
    int PageNumber { get; }

    /// <summary>
    /// Gets the page size.
    /// </summary>
    int PageSize { get; }

    /// <summary>
    /// Gets the total page count.
    /// </summary>
    int PageCount { get; }
}

/// <summary>
/// Represents a list result with a total item count.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public interface IListResultWithTotal<out T>
{
    /// <summary>
    /// Gets the result items.
    /// </summary>
    IReadOnlyList<T> Data { get; }

    /// <summary>
    /// Gets the total item count.
    /// </summary>
    int TotalCount { get; }
}

/// <summary>
/// Extension methods for list results.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    /// Gets an enumerator for the result data.
    /// </summary>
    /// <typeparam name="T">The item type.</typeparam>
    /// <param name="listResult">The list result.</param>
    /// <returns>An enumerator for the result data.</returns>
    public static IEnumerator<T> GetEnumerator<T>(this IListResultWithTotal<T> listResult)
        => listResult.Data.GetEnumerator();
}

/// <summary>
/// Represents a list result with total item count.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
public class ListResultWithTotal<T> : IListResultWithTotal<T>
{
    /// <summary>
    /// Gets an empty list result.
    /// </summary>
    public static readonly ListResultWithTotal<T> Empty = new();

    private IReadOnlyList<T> _data = Array.Empty<T>();

    /// <inheritdoc />
    public IReadOnlyList<T> Data
    {
        get => _data;
        set => _data = Guard.NotNull(value, nameof(value));
    }

    /// <inheritdoc />
    public int TotalCount { get; set; }
}

/// <summary>
/// Represents a paged list result.
/// </summary>
/// <typeparam name="T">The item type.</typeparam>
[Serializable]
public class PagedListResult<T> : IPagedListResult<T>
{
    /// <summary>
    /// Gets an empty paged list result.
    /// </summary>
    public static readonly PagedListResult<T> Empty = new();

    private IReadOnlyList<T> _data = Array.Empty<T>();

    /// <inheritdoc />
    public IReadOnlyList<T> Data
    {
        get => _data;
        set => _data = Guard.NotNull(value, nameof(value));
    }

    private int _pageNumber = 1;

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
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

    /// <inheritdoc />
    public int PageCount => (_totalCount + _pageSize - 1) / _pageSize;

    /// <summary>
    /// Gets the item at the specified index in the current page.
    /// </summary>
    /// <param name="index">The zero-based item index.</param>
    /// <returns>The item at the specified index.</returns>
    public T this[int index] => Data[index];

    /// <inheritdoc />
    public int Count => Data.Count;
}
