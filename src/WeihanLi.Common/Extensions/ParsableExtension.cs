// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

#if NET
using WeihanLi.Common;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class ParsableExtension
{
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
