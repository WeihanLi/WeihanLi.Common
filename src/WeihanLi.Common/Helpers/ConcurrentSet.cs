using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WeihanLi.Common.Helpers
{
    public sealed class ConcurrentSet<T> : IReadOnlyCollection<T>, ICollection<T>
    {
        private readonly ConcurrentDictionary<T, bool> _dictionary = new();

        public bool IsEmpty => _dictionary.IsEmpty;

        public int Count => _dictionary.Count;

        public bool IsReadOnly => false;

        public bool Contains(T item) => _dictionary.ContainsKey(item);

        public bool TryAdd(T t) => _dictionary.TryAdd(t, false);

        public bool TryRemove(T t) => _dictionary.TryRemove(t, out _);

        public ICollection<T> Values() => _dictionary.Keys;

        public void Clear() => _dictionary.Clear();

        public IEnumerator<T> GetEnumerator()
        {
            return _dictionary.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            TryAdd(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _dictionary.Keys.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return TryRemove(item);
        }
    }
}
