using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WeihanLi.Common.Models;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions
{
    public static class EnumerableExtension
    {
        public static void ForEach<T>([NotNull]this IEnumerable<T> ts, Action<T> action)
        {
            foreach (var t in ts)
            {
                action(t);
            }
        }

        public static void ForEach<T>([NotNull]this IEnumerable<T> ts, Action<T, int> action)
        {
            var i = 0;
            foreach (var t in ts)
            {
                action(t, i++);
            }
        }

        public static async Task ForEachAsync<T>([NotNull]this IEnumerable<T> ts, Func<T, Task> action)
        {
            foreach (var t in ts)
            {
                await action(t);
            }
        }

        public static async Task ForEachAsync<T>([NotNull]this IEnumerable<T> ts, Func<T, int, Task> action)
        {
            var i = 0;
            foreach (var t in ts)
            {
                await action(t, i++);
            }
        }

        /// <summary>
        ///     A T[] extension method that converts an array to a read only.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <returns>A list of.</returns>
        public static ReadOnlyCollection<T> AsReadOnly<T>([NotNull]this IEnumerable<T> @this)
        {
            return Array.AsReadOnly(@this.ToArray());
        }

        /// <summary>
        ///     An IEnumerable&lt;T&gt; extension method that queries if a null or is empty.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The collection to act on.</param>
        /// <returns>true if a null or is t>, false if not.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> @this)
        {
            return @this == null || !@this.Any();
        }

        /// <summary>
        ///     An IEnumerable&lt;T&gt; extension method that queries if a not null or is empty.
        /// </summary>
        /// <typeparam name="T">Generic type parameter.</typeparam>
        /// <param name="this">The collection to act on.</param>
        /// <returns>true if a not null or is t>, false if not.</returns>
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> @this)
        {
            return @this != null && @this.Any();
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
        public static string StringJoin<T>([NotNull]this IEnumerable<T> @this, string separator)
        {
            return string.Join(separator, @this);
        }

        public static IEnumerable<TSource> Prepend<TSource>([NotNull]this IEnumerable<TSource> source, TSource value)
        {
            yield return value;

            foreach (var element in source)
            {
                yield return element;
            }
        }

        public static IEnumerable<TSource> Append<TSource>([NotNull]this IEnumerable<TSource> source, TSource value)
        {
            foreach (var element in source)
            {
                yield return element;
            }

            yield return value;
        }

        #region Split

        /// <summary>
        /// 将一维集合分割成二维集合
        /// </summary>
        /// <param name="source">source</param>
        /// <param name="batchSize">每个一维集合的数量</param>
        public static IEnumerable<T[]> Split<T>(this IEnumerable<T> source, int batchSize)
        {
            using (var enumerator = source.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    yield return Split(enumerator, batchSize).ToArray();
                }
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

        #region PagedListModel

        /// <summary>
        /// ToPagedListModel
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="data">data</param>
        /// <param name="pageNumber">pageNumber</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="totalCount">totalCount</param>
        /// <returns></returns>
        public static PagedListModel<T> ToPagedListModel<T>([NotNull]this IEnumerable<T> data, int pageNumber, int pageSize, int totalCount)
            => new PagedListModel<T>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = data.ToArray()
            };

        /// <summary>
        /// ToPagedListModel
        /// </summary>
        /// <typeparam name="T">Type</typeparam>
        /// <param name="data">data</param>
        /// <param name="pageNumber">pageNumber</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="totalCount">totalCount</param>
        /// <returns></returns>
        public static PagedListModel<T> ToPagedListModel<T>([NotNull]this IReadOnlyList<T> data, int pageNumber, int pageSize, int totalCount)
            => new PagedListModel<T>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Data = data
            };

        #endregion PagedListModel
    }
}
