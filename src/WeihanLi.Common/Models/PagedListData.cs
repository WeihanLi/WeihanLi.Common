using JetBrains.Annotations;
using System.Collections.Generic;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Models
{
    /// <summary>
    /// PagedListData,
    /// data with page info
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public interface IPagedListData<out T>
    {
        /// <summary>
        /// Data
        /// </summary>
        IReadOnlyList<T> Data { get; }

        int Count { get; }

        /// <summary>
        /// PageNumber
        /// </summary>
        int PageNum { get; }

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

    public class PagedListData<T> : IPagedListData<T>
    {
        public static readonly IPagedListData<T> Empty = new PagedListData<T>();

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

        private int _pageNum = 1;

        public int PageNum
        {
            get => _pageNum;
            set
            {
                if (value > 0)
                {
                    _pageNum = value;
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
