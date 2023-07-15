using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace WeihanLi.Common;

public static class Guard
{
    [return: NotNull]
    public static T NotNull<T>([NotNull] T? t,
           [CallerArgumentExpression(nameof(t))]
            string? paramName = default)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(t, paramName);
#else
        if (t is null)
        {
            throw new ArgumentNullException(paramName);
        }
#endif
        return t;
    }

    [return: NotNull]
    public static string NotNullOrEmpty([NotNull] string? str,
        [CallerArgumentExpression(nameof(str))]
            string? paramName = null)
    {
#if NET7_0_OR_GREATER
        ArgumentException.ThrowIfNullOrEmpty(str, paramName);
#else
        NotNull(str, paramName);
        if (str.Length == 0)
        {
            throw new ArgumentException("The argument can not be Empty", paramName);
        }
#endif
        return str;
    }

    [return: NotNull]
    public static string NotNullOrWhiteSpace([NotNull] string? str,
        [CallerArgumentExpression(nameof(str))] string? paramName = null)
    {
#if NET8_0_OR_GREATER
        ArgumentException.ThrowIfNullOrWhiteSpace(str, paramName);
#else
        NotNull(str, paramName);
        if (string.IsNullOrWhiteSpace(str))
        {
            throw new ArgumentException("The argument can not be WhiteSpace", paramName);
        }
#endif
        return str;
    }

    [return: NotNull]
    public static ICollection<T> NotEmpty<T>([NotNull] ICollection<T> collection, [CallerArgumentExpression(nameof(collection))] string? paramName = null)
    {
        NotNull(collection, paramName);
        if (collection.Count == 0)
        {
            throw new ArgumentException("The collection could not be empty", paramName);
        }
        return collection;
    }

    public static T Ensure<T>(Func<T, bool> condition, T t, [CallerArgumentExpression(nameof(t))] string? paramName = null)
    {
        NotNull(condition);
        if (!condition(t))
        {
            throw new ArgumentException("The argument does not meet condition", paramName);
        }
        return t;
    }

    public static async Task<T> EnsureAsync<T>(Func<T, Task<bool>> condition, T t, [CallerArgumentExpression(nameof(t))] string? paramName = null)
    {
        NotNull(condition);
        if (!await condition(t))
        {
            throw new ArgumentException("The argument does not meet condition", paramName);
        }
        return t;
    }

#if ValueTaskSupport
    public static async Task<T> EnsureAsync<T>(Func<T, ValueTask<bool>> condition, T t, [CallerArgumentExpression(nameof(t))] string? paramName = null)
    {
        NotNull(condition);
        if (!await condition(t))
        {
            throw new ArgumentException("The argument does not meet condition", paramName);
        }
        return t;
    }
#endif
}
