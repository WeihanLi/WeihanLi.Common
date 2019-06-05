using System;
using System.Collections;
using System.Collections.Generic;

namespace WeihanLi.Common.Models
{
    /// <summary>
    /// IPagedListModel
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    public interface IPagedListModel<out T> : IReadOnlyList<T>
    {
        /// <summary>
        /// Data
        /// </summary>
        IReadOnlyList<T> Data { get; }

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
    }

    /// <inheritdoc />
    /// <summary>
    /// 分页Model
    /// </summary>
    /// <typeparam name="T">Type</typeparam>
    [Serializable]
    public class PagedListModel<T> : IPagedListModel<T>
    {
        public IReadOnlyList<T> Data { get; set; }

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

        public int PageCount => Convert.ToInt32(Math.Ceiling(_totalCount * 1.0 / _pageSize));

        public IEnumerator<T> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        public T this[int index] => Data[index];

        public int Count => Data.Count;
    }
}
