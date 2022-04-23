using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace WeihanLi.Common;

public static class Guard
{
    public static T NotNull<T>([NotNull] T? t,
           [CallerArgumentExpression("t")]
            string? paramName = default)
    {
#if NET6_0_OR_GREATER
        ArgumentNullException.ThrowIfNull(t, paramName);
#else
        if (t is null)
        {
            ThrowNullException(paramName);
        }
#endif
        return t;
    }

    [DoesNotReturn]
    private static void ThrowNullException(string? paramName) =>
        throw new ArgumentNullException(paramName);

    public static string NotNullOrEmpty([NotNull] string? str,
        [CallerArgumentExpression("str")]
            string? paramName = null)
    {
        NotNull(str, paramName);
        if (str.Length == 0)
        {
            throw new ArgumentException("The argument can not be Empty", paramName);
        }
        return str;
    }

    public static string NotNullOrWhiteSpace([NotNull] string? str,
        [CallerArgumentExpression("str")] string? paramName = null)
    {
        NotNull(str, paramName);
        if (str.Trim().Length == 0)
        {
            throw new ArgumentException("The argument can not be WhiteSpace", paramName);
        }
        return str;
    }
}
