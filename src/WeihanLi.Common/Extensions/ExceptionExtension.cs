using JetBrains.Annotations;
using System;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions
{
    /// <summary>
    /// ExceptionExtension
    /// </summary>
    public static class ExceptionExtension
    {
        /// <summary>
        /// get inner exception of AggregateException
        /// </summary>
        /// <param name="exception">origin exception</param>
        /// <param name="depth">depth</param>
        /// <returns>inner exception</returns>
        public static Exception Unwrap([NotNull]this Exception exception, int depth = 16)
        {
            while (exception is AggregateException && exception.InnerException != null && depth-- > 0)
            {
                exception = exception.InnerException;
            }
            return exception;
        }
    }
}
