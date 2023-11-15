using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using WeihanLi.Common.DependencyInjection;

namespace WeihanLi.Common;

public static class CacheUtil
{
    private static readonly ConcurrentDictionary<Type, PropertyInfo[]> TypePropertyCache = new();
    private static readonly ConcurrentDictionary<Type, FieldInfo[]> TypeFieldCache = new();
        
    public static PropertyInfo[] GetTypeProperties([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]Type type)
    {
        Guard.NotNull(type);
        return TypePropertyCache.GetOrAdd(type, _ => type.GetProperties());
    }
    
    public static FieldInfo[] GetTypeFields([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]Type type)
    {
        Guard.NotNull(type);
        return TypeFieldCache.GetOrAdd(type, _ => type.GetFields());
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
