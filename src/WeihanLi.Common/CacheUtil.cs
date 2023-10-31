using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using WeihanLi.Common.DependencyInjection;

namespace WeihanLi.Common;

public static class CacheUtil
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> TypePropertyCache = new();
    private static readonly ConcurrentDictionary<Type, FieldInfo[]> TypeFieldCache = new();

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
    public static PropertyInfo[] GetTypeProperties(Type type)
    {
        Guard.NotNull(type);
        return TypePropertyCache.GetOrAdd(type, t => t.GetProperties());
    }

    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
    public static FieldInfo[] GetTypeFields(Type type)
    {
        Guard.NotNull(type);
        return TypeFieldCache.GetOrAdd(type, t => t.GetFields());
    }

    internal static readonly ConcurrentDictionary<Type, MethodInfo[]> TypeMethodCache = new();

    internal static readonly ConcurrentDictionary<Type, Func<ServiceContainer, object>> TypeNewFuncCache = new();

    internal static readonly ConcurrentDictionary<Type, ConstructorInfo?> TypeConstructorCache = new();

    internal static readonly ConcurrentDictionary<Type, Func<object>> TypeEmptyConstructorFuncCache = new();

    internal static readonly ConcurrentDictionary<Type, Func<object?[], object>> TypeConstructorFuncCache = new();

    internal static readonly ConcurrentDictionary<PropertyInfo, Func<object, object?>?> PropertyValueGetters = new();

    internal static readonly ConcurrentDictionary<PropertyInfo, Action<object, object?>?> PropertyValueSetters = new();
}

internal static class StrongTypedCache<T>
{
    public static readonly ConcurrentDictionary<PropertyInfo, Func<T, object?>?> PropertyValueGetters = new();

    public static readonly ConcurrentDictionary<PropertyInfo, Action<T, object?>?> PropertyValueSetters = new();
}
