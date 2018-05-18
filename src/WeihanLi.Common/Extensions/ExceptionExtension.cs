using System;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions
{
    /// <summary>
    /// ExceptionExtension
    /// </summary>
    public static class ExceptionExtension
    {
        /// <summary>
        /// 获取真实的异常信息
        /// </summary>
        /// <param name="ex">原始异常</param>
        /// <returns>真实异常</returns>
        public static Exception Unwrap([NotNull]this Exception ex)
        {
            var counter = 64;
            while (counter-- > 0)
            {
                if (ex is AggregateException && ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                else
                {
                    break;
                }
            }
            return ex;
        }
    }
}
