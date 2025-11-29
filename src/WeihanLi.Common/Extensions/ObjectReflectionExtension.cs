// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using WeihanLi.Common;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

/// <summary>
/// Extensions that require reflection and are not AOT-compatible
/// </summary>
public static class ObjectReflectionExtension
{
    #region Enum

    /// <summary>
    /// A T extension method to determines whether the object is equal to any of the provided values.
    /// </summary>
    public static bool In(this Enum @this, params Enum[] values)
    {
        return Array.IndexOf(values, @this) >= 0;
    }

    /// <summary>
    /// An object extension method that gets description attribute.
    /// </summary>
    public static string GetDescription(this Enum value)
    {
        var stringValue = value.ToString();
        var attr = value.GetType().GetField(stringValue)?
            .GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description ?? stringValue;
    }

    #endregion Enum

    #region object

    /// <summary>
    /// An object extension method that converts the @this to an or default.
    /// </summary>
    public static T? AsOrDefault<T>(this object? @this)
    {
        if (@this is null) return default;
        try { return (T)@this; }
        catch { return default; }
    }

    /// <summary>
    /// An object extension method that converts the @this to an or default.
    /// </summary>
    public static T AsOrDefault<T>(this object? @this, T defaultValue)
    {
        if (@this is null) return defaultValue;
        try { return (T)@this; }
        catch { return defaultValue; }
    }

    /// <summary>
    /// An object extension method that converts the @this to an or default.
    /// </summary>
    public static T AsOrDefault<T>(this object? @this, Func<T> defaultValueFactory)
    {
        if (@this is null) return defaultValueFactory();
        try { return (T)@this; }
        catch { return defaultValueFactory(); }
    }

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated.")]
    public static T To<T>(this object? @this)
    {
#nullable disable
        if (@this == null || @this == DBNull.Value) return (T)(object)null;
#nullable restore
        var targetType = typeof(T).Unwrap();
        var sourceType = @this.GetType().Unwrap();
        if (sourceType == targetType) return (T)@this;
        var converter = TypeDescriptor.GetConverter(sourceType);
        if (converter.CanConvertTo(targetType)) return (T)converter.ConvertTo(@this, targetType)!;
        converter = TypeDescriptor.GetConverter(targetType);
        if (converter.CanConvertFrom(sourceType)) return (T)converter.ConvertFrom(@this)!;
        return (T)Convert.ChangeType(@this, targetType);
    }

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated.")]
    public static object? To(this object? @this, Type type)
    {
        if (@this == null || @this == DBNull.Value) return null;
        var targetType = type.Unwrap();
        var sourceType = @this.GetType().Unwrap();
        if (sourceType == targetType) return @this;
        var converter = TypeDescriptor.GetConverter(sourceType);
        if (converter.CanConvertTo(targetType)) return converter.ConvertTo(@this, targetType);
        converter = TypeDescriptor.GetConverter(targetType);
        if (converter.CanConvertFrom(sourceType)) return converter.ConvertFrom(@this);
        return Convert.ChangeType(@this, targetType);
    }

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated.")]
    public static T ToOrDefault<T>(this object? @this, Func<object?, T> defaultValueFactory)
    {
        try { return (T)@this.To(typeof(T))!; }
        catch { return defaultValueFactory(@this); }
    }

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated.")]
    public static T ToOrDefault<T>(this object? @this, Func<T> defaultValueFactory) => @this.ToOrDefault(_ => defaultValueFactory());

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated.")]
    public static object? ToOrDefault(this object? @this, Type type)
    {
        Guard.NotNull(type, nameof(type));
        try { return @this.To(type); }
        catch { return type.GetDefaultValue(); }
    }

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated.")]
    public static T? ToOrDefault<T>(this object? @this) => @this.ToOrDefault(_ => default(T));

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated.")]
    public static T ToOrDefault<T>(this object? @this, T defaultValue) => @this.ToOrDefault(_ => defaultValue);

    public static T Chain<T>(this T @this, Action<T>? action) { action?.Invoke(@this); return @this; }

    public static TResult? GetValueOrDefault<T, TResult>(this T @this, Func<T, TResult> func)
    {
        try { return func(@this); }
        catch { return default; }
    }

    public static TResult GetValueOrDefault<T, TResult>(this T @this, Func<T, TResult> func, TResult defaultValue)
    {
        try { return func(@this); }
        catch { return defaultValue; }
    }

    public static TResult Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, TResult catchValue)
    {
        try { return tryFunction(@this); }
        catch { return catchValue; }
    }

    public static TResult Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, Func<TType, TResult> catchValueFactory)
    {
        try { return tryFunction(@this); }
        catch { return catchValueFactory(@this); }
    }

    public static bool Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, out TResult? result)
    {
        try { result = tryFunction(@this); return true; }
        catch { result = default; return false; }
    }

    public static bool Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, TResult catchValue, out TResult result)
    {
        try { result = tryFunction(@this); return true; }
        catch { result = catchValue; return false; }
    }

    public static bool Try<TType, TResult>(this TType @this, Func<TType, TResult> tryFunction, Func<TType, TResult> catchValueFactory, out TResult result)
    {
        try { result = tryFunction(@this); return true; }
        catch { result = catchValueFactory(@this); return false; }
    }

    public static bool Try<TType>(this TType @this, Action<TType> tryAction)
    {
        try { tryAction(@this); return true; }
        catch { return false; }
    }

    public static bool Try<TType>(this TType @this, Action<TType> tryAction, Action<TType> catchAction)
    {
        try { tryAction(@this); return true; }
        catch { catchAction(@this); return false; }
    }

    public static bool InRange<T>(this T @this, T minValue, T maxValue) where T : IComparable<T>
        => @this.CompareTo(minValue) >= 0 && @this.CompareTo(maxValue) <= 0;

    public static bool IsDefault<T>(this T source) => source is null || source.Equals(default(T));

    public static IDictionary<string, object?> ParseParamDictionary(this object? paramInfo)
    {
        var paramDic = new Dictionary<string, object?>();
        if (paramInfo is null) return paramDic;

        var type = paramInfo.GetType();
        if (type.IsValueTuple())
        {
            var fields = CacheUtil.GetTypeFields(type);
            foreach (var field in fields)
                paramDic[field.Name] = field.GetValue(paramInfo);
        }
        else if (paramInfo is IDictionary<string, object?> paramDictionary)
        {
            return paramDictionary;
        }
        else
        {
            var properties = CacheUtil.GetTypeProperties(type);
            foreach (var property in properties)
                if (property.CanRead)
                    paramDic[property.Name] = property.GetValueGetter()?.Invoke(paramInfo);
        }

        return paramDic;
    }
    #endregion object

    #region Type

    public static T? CreateInstance<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] this Type @this, params object?[]? args) 
        => (T?)Activator.CreateInstance(@this, args);

    public static bool HasEmptyConstructor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] this Type type)
        => Guard.NotNull(type, nameof(type)).GetConstructors(BindingFlags.Instance).Any(c => c.GetParameters().Length == 0);

    public static object? GetDefaultValue(this Type type)
    {
        Guard.NotNull(type, nameof(type));
        return type.IsValueType && type != typeof(void)
            ? DefaultValues.GetOrAdd(type, Activator.CreateInstance)
            : null;
    }

    private static readonly ConcurrentDictionary<Type, object?> DefaultValues = new();

    #endregion Type
}
