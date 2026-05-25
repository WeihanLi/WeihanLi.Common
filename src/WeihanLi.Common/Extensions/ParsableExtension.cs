// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

#if NET
using WeihanLi.Common;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class ParsableExtension
{
    /// <summary>
    /// Parse string to specific type instance.
    /// </summary>
    /// <param name="this">The string text.</param>
    /// <param name="formatProvider">An object that provides culture-specific formatting information.</param>
    /// <typeparam name="T">The destination type.</typeparam>
    /// <returns>The parsed value of type T.</returns>
    public static T To<T>(this string @this, IFormatProvider? formatProvider = null)
        where T : IParsable<T>
    {
        return T.Parse(@this, formatProvider);
    }

    /// <summary>
    /// Parse string to specific type instance.
    /// </summary>
    /// <param name="this">The string text.</param>
    /// <param name="formatProvider">An object that provides culture-specific formatting information.</param>
    /// <param name="defaultValue">The default value.</param>
    /// <typeparam name="T">The destination type.</typeparam>
    /// <returns>The parsed value of type T, or the default value if parsing fails.</returns>
    public static T? ToOrDefault<T>(this string @this, IFormatProvider? formatProvider = null, T? defaultValue = default)
        where T : IParsable<T>
    {
        return T.TryParse(@this, formatProvider, out var result) ? result : defaultValue;
    }

    /// <summary>
    /// Split comma separated string to T array.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    /// <param name="str">The string.</param>
    /// <param name="splitOptions">The split options.</param>
    /// <returns>The parsed array.</returns>
    public static T[] SplitArray<T>(this string? str, StringSplitOptions splitOptions = StringSplitOptions.None)
        where T : IParsable<T>
        => SplitArray<T>(str, [','], splitOptions);

    /// <summary>
    /// Split specific separator separated string to T array.
    /// </summary>
    /// <typeparam name="T">Type.</typeparam>
    /// <param name="str">The string.</param>
    /// <param name="separators">Separators.</param>
    /// <param name="splitOptions">The split options.</param>
    /// <returns>The parsed array.</returns>
    public static T[] SplitArray<T>(this string? str, char[] separators, StringSplitOptions splitOptions = StringSplitOptions.None)
        where T : IParsable<T>
    {
        if (string.IsNullOrWhiteSpace(str))
        {
            return [];
        }

        return Guard.NotNull(str)
            .Split(separators, splitOptions)
            .Select(s => s.To<T>())
            .ToArray();
    }
}
#endif
