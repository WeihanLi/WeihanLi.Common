using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace WeihanLi.Common
{
    public static class CacheUtil
    {
        /// <summary>
        /// TypePropertyCache
        /// </summary>
        public static readonly ConcurrentDictionary<Type, PropertyInfo[]> TypePropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

        internal static readonly ConcurrentDictionary<PropertyInfo, Func<object, object>> PropertyValueGetters = new ConcurrentDictionary<PropertyInfo, Func<object, object>>();

        internal static readonly ConcurrentDictionary<PropertyInfo, Action<object, object>> PropertyValueSetters = new ConcurrentDictionary<PropertyInfo, Action<object, object>>();
    }

    internal static class PropertyInfoCache<T>
    {
        public static ConcurrentDictionary<PropertyInfo, Func<T, object>> PropertyValueGetters { get; } = new ConcurrentDictionary<PropertyInfo, Func<T, object>>();

        public static ConcurrentDictionary<PropertyInfo, Action<T, object>> PropertyValueSetters { get; } = new ConcurrentDictionary<PropertyInfo, Action<T, object>>();
    }
}
