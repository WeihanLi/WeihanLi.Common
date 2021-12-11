using System;
using System.Runtime.CompilerServices;

namespace WeihanLi.Common;

public static class Guard
{
    public static T NotNull<T>(T? t,
           [CallerArgumentExpression("t")]
            string? paramName = default)
    {
        if (t is null)
        {
            throw new ArgumentNullException(paramName);
        }
        return t;
    }

    public static string NotNullOrEmpty(string? str,
        [CallerArgumentExpression("str")]
            string? paramName = default)
    {
        NotNull(str, paramName);
        if (string.IsNullOrEmpty(str))
        {
            throw new ArgumentException("The argument is IsNullOrEmpty", paramName);
        }
        return str!;
    }
}
