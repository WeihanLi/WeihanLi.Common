using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions
{
    /// <summary>
    /// DictionaryExtension
    /// </summary>
    public static class DictionaryExtension
    {
        /// <summary>
        /// 根据key获取Dictionary中元素
        /// </summary>
        /// <typeparam name="TKey">key类型</typeparam>
        /// <typeparam name="TValue">value类型</typeparam>
        /// <param name="dictionary">字典</param>
        /// <param name="key">key</param>
        /// <param name="value">value</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static bool TryGetValue<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, TKey key, out TValue value, TValue defaultValue)
        {
            var result = dictionary.TryGetValue(key, out value);
            if (!result)
            {
                value = defaultValue;
            }
            return result;
        }

        /// <summary>
        ///     An IDictionary&lt;TKey,TValue&gt; extension method that adds if not contains key.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool AddIfNotContainsKey<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> @this, TKey key, TValue value)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(key, value);
                return true;
            }

            return false;
        }

        /// <summary>
        ///     An IDictionary&lt;TKey,TValue&gt; extension method that adds if not contains key.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="key">The key.</param>
        /// <param name="valueFactory">The value factory.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool AddIfNotContainsKey<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> @this, TKey key, Func<TValue> valueFactory)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(key, valueFactory());
                return true;
            }

            return false;
        }

        /// <summary>
        ///     An IDictionary&lt;TKey,TValue&gt; extension method that adds if not contains key.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="key">The key.</param>
        /// <param name="valueFactory">The value factory.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool AddIfNotContainsKey<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> @this, TKey key, Func<TKey, TValue> valueFactory)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(key, valueFactory(key));
                return true;
            }

            return false;
        }

        /// <summary>
        ///     Adds a key/value pair to the IDictionary&lt;TKey, TValue&gt; if the key does not already exist.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value to be added, if the key does not already exist.</param>
        /// <returns>
        ///     The value for the key. This will be either the existing value for the key if the key is already in the
        ///     dictionary, or the new value if the key was not in the dictionary.
        /// </returns>
        public static TValue GetOrAdd<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> @this, TKey key, TValue value)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(key, value);
            }
            return @this[key];
        }

        /// <summary>
        ///     Adds a key/value pair to the IDictionary&lt;TKey, TValue&gt; by using the specified function, if the key does
        ///     not already exist.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="valueFactory">TThe function used to generate a value for the key.</param>
        /// <returns>
        ///     The value for the key. This will be either the existing value for the key if the key is already in the
        ///     dictionary, or the new value for the key as returned by valueFactory if the key was not in the dictionary.
        /// </returns>
        public static TValue GetOrAdd<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> @this, TKey key, Func<TKey, TValue> valueFactory)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(new KeyValuePair<TKey, TValue>(key, valueFactory(key)));
            }

            return @this[key];
        }

        /// <summary>
        ///     Uses the specified functions to add a key/value pair to the IDictionary&lt;TKey, TValue&gt; if the key does
        ///     not already exist, or to update a key/value pair in the IDictionary&lt;TKey, TValue&gt;> if the key already
        ///     exists.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="key">The key to be added or whose value should be updated.</param>
        /// <param name="value">The value to be added or updated.</param>
        /// <returns>The new value for the key.</returns>
        public static TValue AddOrUpdate<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> @this, TKey key, TValue value)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(new KeyValuePair<TKey, TValue>(key, value));
            }
            else
            {
                @this[key] = value;
            }

            return @this[key];
        }

        /// <summary>
        ///     Uses the specified functions to add a key/value pair to the IDictionary&lt;TKey, TValue&gt; if the key does
        ///     not already exist, or to update a key/value pair in the IDictionary&lt;TKey, TValue&gt;> if the key already
        ///     exists.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="key">The key to be added or whose value should be updated.</param>
        /// <param name="addValue">The value to be added for an absent key.</param>
        /// <param name="updateValueFactory">
        ///     The function used to generate a new value for an existing key based on the key's
        ///     existing value.
        /// </param>
        /// <returns>
        ///     The new value for the key. This will be either be addValue (if the key was absent) or the result of
        ///     updateValueFactory (if the key was present).
        /// </returns>
        public static TValue AddOrUpdate<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> @this, TKey key, TValue addValue, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(new KeyValuePair<TKey, TValue>(key, addValue));
            }
            else
            {
                @this[key] = updateValueFactory(key, @this[key]);
            }

            return @this[key];
        }

        /// <summary>
        ///     Uses the specified functions to add a key/value pair to the IDictionary&lt;TKey, TValue&gt; if the key does
        ///     not already exist, or to update a key/value pair in the IDictionary&lt;TKey, TValue&gt;> if the key already
        ///     exists.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="key">The key to be added or whose value should be updated.</param>
        /// <param name="addValueFactory">The function used to generate a value for an absent key.</param>
        /// <param name="updateValueFactory">
        ///     The function used to generate a new value for an existing key based on the key's
        ///     existing value.
        /// </param>
        /// <returns>
        ///     The new value for the key. This will be either be the result of addValueFactory (if the key was absent) or
        ///     the result of updateValueFactory (if the key was present).
        /// </returns>
        public static TValue AddOrUpdate<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> @this, TKey key, Func<TKey, TValue> addValueFactory, Func<TKey, TValue, TValue> updateValueFactory)
        {
            if (!@this.ContainsKey(key))
            {
                @this.Add(new KeyValuePair<TKey, TValue>(key, addValueFactory(key)));
            }
            else
            {
                @this[key] = updateValueFactory(key, @this[key]);
            }

            return @this[key];
        }

        /// <summary>
        ///     An IDictionary&lt;TKey,TValue&gt; extension method that removes if contains key.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="key">The key.</param>
        public static void RemoveIfContainsKey<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> @this, TKey key)
        {
            if (@this.ContainsKey(key))
            {
                @this.Remove(key);
            }
        }

        /// <summary>
        ///     An IDictionary&lt;TKey,TValue&gt; extension method that converts the @this to a sorted dictionary.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <returns>@this as a SortedDictionary&lt;TKey,TValue&gt;</returns>
        public static SortedDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> @this)
        {
            return new(@this);
        }

        /// <summary>
        ///     An IDictionary&lt;TKey,TValue&gt; extension method that converts the @this to a sorted dictionary.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="comparer">The comparer.</param>
        /// <returns>@this as a SortedDictionary&lt;TKey,TValue&gt;</returns>
        public static SortedDictionary<TKey, TValue> ToSortedDictionary<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> @this, IComparer<TKey> comparer)
        {
            return new(@this, comparer);
        }

        /// <summary>
        ///     An IDictionary&lt;TKey,TValue&gt; extension method that query if '@this' contains any key.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="keys">A variable-length parameters list containing keys.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool ContainsAnyKey<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> @this, params TKey[] keys)
        {
            foreach (var value in keys)
            {
                if (@this.ContainsKey(value))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///     An IDictionary&lt;TKey,TValue&gt; extension method that query if '@this' contains all key.
        /// </summary>
        /// <typeparam name="TKey">Type of the key.</typeparam>
        /// <typeparam name="TValue">Type of the value.</typeparam>
        /// <param name="this">The @this to act on.</param>
        /// <param name="keys">A variable-length parameters list containing keys.</param>
        /// <returns>true if it succeeds, false if it fails.</returns>
        public static bool ContainsAllKey<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> @this, params TKey[] keys)
        {
            foreach (var value in keys)
            {
                if (!@this.ContainsKey(value))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///     An IDictionary&lt;string,string&gt; extension method that converts the @this to a name value collection.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <returns>@this as a NameValueCollection.</returns>
        public static NameValueCollection ToNameValueCollection(this IDictionary<string, string> @this)
        {
            if (@this == null)
            {
                return new NameValueCollection();
            }
            var col = new NameValueCollection();
            foreach (var item in @this)
            {
                col.Add(item.Key, item.Value);
            }
            return col;
        }

        public static NameValueCollection ToNameValueCollection(this IEnumerable<KeyValuePair<string, string>> source)
        {
            if (source == null)
            {
                return new NameValueCollection();
            }

            var collection = new NameValueCollection();

            foreach (var item in source)
            {
                if (string.IsNullOrWhiteSpace(item.Key))
                {
                    continue;
                }
                collection.Add(item.Key, item.Value);
            }

            return collection;
        }

        /// <summary>
        ///     An IDictionary&lt;string,object&gt; extension method that converts this object to a database parameters.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="command">The command.</param>
        /// <returns>The given data converted to a DbParameter[].</returns>
        public static DbParameter[] ToDbParameters([NotNull] this IDictionary<string, object> @this, DbCommand command)
        {
            return @this.Select(x =>
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = x.Key;
                parameter.Value = x.Value;
                return parameter;
            }).ToArray();
        }

        /// <summary>
        ///     An IDictionary&lt;string,object&gt; extension method that converts this object to a database parameters.
        /// </summary>
        /// <param name="this">The @this to act on.</param>
        /// <param name="connection">The connection.</param>
        /// <returns>The given data converted to a DbParameter[].</returns>
        public static DbParameter[] ToDbParameters([NotNull] this IDictionary<string, object> @this, DbConnection connection)
        {
            var command = connection.CreateCommand();

            return @this.Select(x =>
            {
                var parameter = command.CreateParameter();
                parameter.ParameterName = x.Key;
                parameter.Value = x.Value;
                return parameter;
            }).ToArray();
        }

        /// <summary>
        /// IDictionary to dataTable
        /// </summary>
        /// <param name="dictionary">IDictionary</param>
        /// <returns></returns>
        public static DataTable ToDataTable([NotNull] this IDictionary<string, object> dictionary)
        {
            if (null == dictionary)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
            var dataTable = new DataTable();
            if (dictionary.Keys.Count == 0)
            {
                return dataTable;
            }
            dataTable.Columns.AddRange(dictionary.Keys.Select(key => new DataColumn(key, dictionary[key].GetType())).ToArray());
            foreach (var key in dictionary.Keys)
            {
                var row = dataTable.NewRow();
                row[key] = dictionary[key];
                dataTable.Rows.Add(row);
            }
            return dataTable;
        }

        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>([NotNull] this IEnumerable<KeyValuePair<TKey, TValue>> source) => source.ToDictionary(pair => pair.Key, pair => pair.Value);

        public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePair(this NameValueCollection collection)
        {
            if (collection == null || collection.Count == 0)
            {
                yield break;
            }

            foreach (var key in collection.AllKeys)
            {
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                yield return new KeyValuePair<string, string>(key, collection[key]);
            }
        }

        /// <summary>将键值集合转换成字符串，key1=value1&amp;key2=value2，k/v会编码</summary>
        /// <param name="source">数据源</param>
        /// <returns>字符串</returns>
        public static string ToQueryString(this IEnumerable<KeyValuePair<string, string>> source)
        {
            if (source == null)
            {
                return string.Empty;
            }

            var sb = new StringBuilder(1024);

            foreach (var item in source)
            {
                if (string.IsNullOrWhiteSpace(item.Key))
                {
                    continue;
                }
                sb.Append("&");
                sb.Append(item.Key.UrlEncode());
                sb.Append("=");
                if (item.Value != null)
                    sb.Append(item.Value.UrlEncode());
            }

            return sb.Length > 1 ? sb.ToString(1, sb.Length - 1) : "";
        }
    }
}
