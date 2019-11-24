using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace WeihanLi.Common.Models
{
    /// <summary>
    /// IPagedListModel
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public interface IPagedListModel<out T>
    {
        /// <summary>
        /// Data
        /// </summary>
        IReadOnlyList<T> Data { get; }

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
        /// TotalDataCount
        /// </summary>
        int TotalCount { get; set; }

        /// <summary>
        /// PageCount
        /// </summary>
        int PageCount { get; }
    }

    /// <summary>
    /// 分页Model
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    [Serializable]
    public class PagedListModel<T> : IPagedListModel<T>
    {
        public static readonly IPagedListModel<T> Empty = new PagedListModel<T>();

        private IReadOnlyList<T> _data =
#if NET45
    Helpers.ArrayHelper.Empty<T>()
#else
            Array.Empty<T>()
#endif
        ;

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

        public int PageCount => ((_totalCount % _pageSize) == 0)
            ? _totalCount / _pageSize
            : Convert.ToInt32(Math.Ceiling(_totalCount * 1.0 / _pageSize));

        public T this[int index] => Data[index];

        public int Count => Data.Count;
    }
}
