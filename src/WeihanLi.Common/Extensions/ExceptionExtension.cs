using System.Diagnostics.CodeAnalysis;
using System.Reflection;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

/// <summary>
/// ExceptionExtension
/// </summary>
public static class ExceptionExtension
{
    /// <summary>
    /// get inner exception of AggregateException
    /// </summary>
    /// <param name="ex">origin exception</param>
    /// <param name="depth">depth</param>
    /// <returns>inner exception</returns>
    [return: NotNullIfNotNull(nameof(ex))]
    public static Exception? Unwrap(this Exception? ex, int depth = 16)
    {
        var exception = ex;
        while (exception is AggregateException or TargetInvocationException
               && exception.InnerException != null
               && depth-- > 0)
        {
            exception = exception.InnerException;
        }
        return exception;
    }

    /// <summary>
    /// Determines whether the provided <paramref name="exception"/> should be considered fatal. An exception
    /// is considered to be fatal if it, or any of the inner exceptions are one of the following type:
    ///
    /// - System.OutOfMemoryException
    /// - System.InsufficientMemoryException
    /// - System.ThreadAbortException
    /// - System.AccessViolationException
    /// - System.StackOverflowException
    /// - System.TypeInitializationException
    /// marked as Fatal.
    /// </summary>
    public static bool IsFatal(this Exception? exception)
    {
        var unwrappedException = exception.Unwrap(256);
        return unwrappedException is OutOfMemoryException and not InsufficientMemoryException
            or ThreadAbortException
            or AccessViolationException
            or StackOverflowException
            or TypeInitializationException;
    }
}
