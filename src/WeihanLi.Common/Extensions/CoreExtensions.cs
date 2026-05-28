// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.ComponentModel;
using WeihanLi.Common;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class CoreExtensions
{
    /// <summary>
    ///     A System.Object extension method that toes the given this.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">this.</param>
    /// <returns>A T.</returns>
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T To<T>(this object? @this)
    {
#nullable disable

        if (@this == null || @this == DBNull.Value)
        {
            return (T)(object)null;
        }
#nullable restore

        var targetType = typeof(T).Unwrap();
        var sourceType = @this.GetType().Unwrap();
        if (sourceType == targetType)
        {
            return (T)@this;
        }
        var converter = TypeDescriptor.GetConverter(sourceType);
        if (converter.CanConvertTo(targetType))
        {
            return (T)converter.ConvertTo(@this, targetType)!;
        }

        converter = TypeDescriptor.GetConverter(targetType);
        if (converter.CanConvertFrom(sourceType))
        {
            return (T)converter.ConvertFrom(@this)!;
        }

        return (T)Convert.ChangeType(@this, targetType);
    }

    /// <summary>
    ///     A System.Object extension method that toes the given this.
    /// </summary>
    /// <param name="this">this.</param>
    /// <param name="type">The type.</param>
    /// <returns>An object.</returns>
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static object? To(this object? @this, Type type)
    {
        if (@this == null || @this == DBNull.Value)
        {
            return null;
        }

        var targetType = type.Unwrap();
        var sourceType = @this.GetType().Unwrap();

        if (sourceType == targetType)
        {
            return @this;
        }

        var converter = TypeDescriptor.GetConverter(sourceType);
        if (converter.CanConvertTo(targetType))
        {
            return converter.ConvertTo(@this, targetType);
        }

        converter = TypeDescriptor.GetConverter(targetType);
        if (converter.CanConvertFrom(sourceType))
        {
            return converter.ConvertFrom(@this);
        }

        return Convert.ChangeType(@this, targetType);
    }

    /// <summary>
    ///     A System.Object extension method that converts this object to an or default.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">this.</param>
    /// <param name="defaultValueFactory">The default value factory.</param>
    /// <returns>The given data converted to a T.</returns>
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T ToOrDefault<T>(this object? @this, Func<object?, T> defaultValueFactory)
    {
        try
        {
            return (T)@this.To(typeof(T))!;
        }
        catch (Exception)
        {
            return defaultValueFactory(@this);
        }
    }

    /// <summary>
    ///     A System.Object extension method that converts this object to an or default.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">this.</param>
    /// <param name="defaultValueFactory">The default value factory.</param>
    /// <returns>The given data converted to a T.</returns>
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T ToOrDefault<T>(this object? @this, Func<T> defaultValueFactory)
    {
        return @this.ToOrDefault(_ => defaultValueFactory());
    }

    /// <summary>
    ///     A System.Object extension method that converts this object to an or default.
    /// </summary>
    /// <param name="this">this.</param>
    /// <param name="type">type</param>
    /// <returns>The given data converted to</returns>
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static object? ToOrDefault(this object? @this, Type type)
    {
        Guard.NotNull(type);
        try
        {
            return @this.To(type);
        }
        catch (Exception)
        {
            return type.GetDefaultValue();
        }
    }

    /// <summary>
    ///     A System.Object extension method that converts this object to an or default.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">this.</param>
    /// <returns>The given data converted to a T.</returns>
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T? ToOrDefault<T>(this object? @this)
    {
        return @this.ToOrDefault(_ => default(T));
    }

    /// <summary>
    ///     A System.Object extension method that converts this object to an or default.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">this.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <returns>The given data converted to a T.</returns>
    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T ToOrDefault<T>(this object? @this, T defaultValue)
    {
        return @this.ToOrDefault(_ => defaultValue);
    }
}
