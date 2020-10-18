using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Models
{
    [Obsolete("Please use IPagedListResult", true)]
    public interface IPagedListModel<out T> : IPagedListResult<T>
    {
    }

    public interface IPagedListResult<out T> : IListResultWithTotalCount<T>
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

    public interface IListResultWithTotalCount<out T>
    {
        IReadOnlyList<T> Data { get; }

        int TotalCount { get; }
    }

    public class ListResultWithTotal<T> : IListResultWithTotalCount<T>
    {
        public static readonly IListResultWithTotalCount<T> Empty = new ListResultWithTotal<T>();

        private IReadOnlyList<T> _data = ArrayHelper.Empty<T>();

        [NotNull]
        public IReadOnlyList<T> Data
        {
            get => _data;
            set
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (value != null)
                {
                    _data = value;
                }
            }
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
        public static readonly IPagedListResult<T> Empty = new PagedListResult<T>();

        private IReadOnlyList<T> _data = ArrayHelper.Empty<T>();

        [NotNull]
        public IReadOnlyList<T> Data
        {
            get => _data;
            set
            {
                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                if (value != null)
                {
                    _data = value;
                }
            }
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
}
