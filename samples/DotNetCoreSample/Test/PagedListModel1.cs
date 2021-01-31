using System;
using System.Collections;
using System.Collections.Generic;

namespace DotNetCoreSample.Test
{
    public class PagedListModel1<T> : IEnumerable<T>
    {
        public IReadOnlyList<T> Data { get; set; } = Array.Empty<T>();

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

        public T this[int index] => Data[index];

        public int Count => Data.Count;

        public IEnumerator<T> GetEnumerator()
        {
            return Data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Data.GetEnumerator();
        }
    }
}
