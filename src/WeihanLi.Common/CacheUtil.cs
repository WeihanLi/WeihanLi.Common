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
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> TypePropertyCache = new ConcurrentDictionary<Type, PropertyInfo[]>();

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

        private static readonly ConcurrentDictionary<Type, FieldInfo[]> TypeFieldCache = new ConcurrentDictionary<Type, FieldInfo[]>();

        internal static readonly ConcurrentDictionary<Type, MethodInfo[]> TypeMethodCache = new ConcurrentDictionary<Type, MethodInfo[]>();

        internal static readonly ConcurrentDictionary<Type, Func<ServiceContainer, object>> TypeNewFuncCache = new ConcurrentDictionary<Type, Func<ServiceContainer, object>>();

        internal static readonly ConcurrentDictionary<Type, ConstructorInfo> TypeConstructorCache = new ConcurrentDictionary<Type, ConstructorInfo>();

        internal static readonly ConcurrentDictionary<Type, Func<object>> TypeEmptyConstructorFuncCache = new ConcurrentDictionary<Type, Func<object>>();

        internal static readonly ConcurrentDictionary<Type, Func<object[], object>> TypeConstructorFuncCache = new ConcurrentDictionary<Type, Func<object[], object>>();

        internal static readonly ConcurrentDictionary<PropertyInfo, Func<object, object>> PropertyValueGetters = new ConcurrentDictionary<PropertyInfo, Func<object, object>>();

        internal static readonly ConcurrentDictionary<PropertyInfo, Action<object, object>> PropertyValueSetters = new ConcurrentDictionary<PropertyInfo, Action<object, object>>();
    }

    internal static class StrongTypedCache<T>
    {
        public static readonly ConcurrentDictionary<PropertyInfo, Func<T, object>> PropertyValueGetters = new ConcurrentDictionary<PropertyInfo, Func<T, object>>();

        public static readonly ConcurrentDictionary<PropertyInfo, Action<T, object>> PropertyValueSetters = new ConcurrentDictionary<PropertyInfo, Action<T, object>>();
    }
}
