using System.Reflection;
using System.Runtime.InteropServices;
using WeihanLi.Common;

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
    public static Exception Unwrap(this Exception ex, int depth = 16)
    {
        Guard.NotNull(ex, nameof(ex));

        var exception = ex;
        while (exception is AggregateException
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
    /// - System.SEHException
    /// - System.StackOverflowException
    /// - System.TypeInitializationException
    /// - Microsoft.PowerApps.CoreFramework.MonitoredException marked as Fatal.
    /// </summary>
    public static bool IsFatal(this Exception exception)
    {
        while (exception != null)
        {
            if ((exception is OutOfMemoryException && !(exception is InsufficientMemoryException)) ||
                (exception is ThreadAbortException) ||
                (exception is AccessViolationException) ||
                (exception is SEHException) ||
                (exception is StackOverflowException) ||
                (exception is TypeInitializationException))
            {
                return true;
            }

            // These exceptions aren't fatal in themselves, but the CLR uses them
            // to wrap other exceptions, so we want to look deeper
            if (exception is TargetInvocationException && exception.InnerException != null)
            {
                exception = exception.InnerException;
            }
            else if (exception is AggregateException aex)
            {
                // AggregateException can contain other AggregateExceptions in its InnerExceptions list so we
                // flatten it first. That will essentially create a list of exceptions from the AggregateException's
                // InnerExceptions property in such a way that any exception other than AggregateException is put
                // into this list. If there is an AggregateException then exceptions from its InnerExceptions list are
                // put into this new list etc. Then a new instance of AggregateException with this flattened list is returned.
                //
                // AggregateException InnerExceptions list is immutable after creation and the walk happens only for
                // the InnerExceptions property of AggregateException and not InnerException of the specific exceptions.
                // This means that the only way to have a circular referencing here is through reflection and forward-
                // reference assignment which would be insane. In such case we would also run into stack overflow
                // when tracing out the exception since AggregateException's ToString does not have any protection there.
                //
                // On that note that's another reason why we want to flatten here as opposed to just let recursion do its magic
                // since in an unlikely case there is a circle we'll get OutOfMemory here instead of StackOverflow which is
                // a lesser of the two evils.

                exception = aex.Unwrap(64);
            }
            else
            {
                break;
            }
        }

        return false;
    }
}
