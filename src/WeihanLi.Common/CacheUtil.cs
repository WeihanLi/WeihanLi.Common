using System;
using System.Collections.Concurrent;
using System.Reflection;
using WeihanLi.Common.DependencyInjection;

namespace WeihanLi.Common
{
    public static class CacheUtil
    {
        /// <summary>
        /// TypePropertyCache
        /// </summary>
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> TypePropertyCache = new();

        public static PropertyInfo[] GetTypeProperties(Type type)
        {
            if (null == type)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return TypePropertyCache.GetOrAdd(type, t => t.GetProperties());
        }

        public static FieldInfo[] GetTypeFields(Type type)
        {
            if (null == type)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return TypeFieldCache.GetOrAdd(type, t => t.GetFields());
        }

        private static readonly ConcurrentDictionary<Type, FieldInfo[]> TypeFieldCache = new();

        internal static readonly ConcurrentDictionary<Type, MethodInfo[]> TypeMethodCache = new();

        internal static readonly ConcurrentDictionary<Type, Func<ServiceContainer, object?>> TypeNewFuncCache = new();

        internal static readonly ConcurrentDictionary<Type, ConstructorInfo?> TypeConstructorCache = new();

        internal static readonly ConcurrentDictionary<Type, Func<object>> TypeEmptyConstructorFuncCache = new();

        internal static readonly ConcurrentDictionary<Type, Func<object?[], object>> TypeConstructorFuncCache = new();

        internal static readonly ConcurrentDictionary<PropertyInfo, Func<object, object?>?> PropertyValueGetters = new();

        internal static readonly ConcurrentDictionary<PropertyInfo, Action<object, object?>?> PropertyValueSetters = new();
    }

    internal static class StrongTypedCache<T>
    {
        public static readonly ConcurrentDictionary<PropertyInfo, Func<T, object>> PropertyValueGetters = new();

        public static readonly ConcurrentDictionary<PropertyInfo, Action<T, object>> PropertyValueSetters = new();
    }
}
