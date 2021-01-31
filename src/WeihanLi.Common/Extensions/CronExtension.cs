using System;
using System.Collections.Generic;
using WeihanLi.Common;
using WeihanLi.Common.Helpers.Cron;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions
{
    public static class CronExtension
    {
        /// <summary>
        /// GetNextOccurrence
        /// </summary>
        /// <param name="expression">cron expression</param>
        /// <returns>next occurrence time</returns>
        public static DateTimeOffset? GetNextOccurrence(this CronExpression? expression)
        {
            return expression?.GetNextOccurrence(DateTimeOffset.UtcNow, TimeZoneInfo.Utc);
        }

        /// <summary>
        /// GetNextOccurrence
        /// </summary>
        /// <param name="expression">cron expression</param>
        /// <param name="timeZoneInfo">timeZoneInfo</param>
        /// <returns>next occurrence time</returns>
        public static DateTimeOffset? GetNextOccurrence(this CronExpression? expression, TimeZoneInfo timeZoneInfo)
        {
            return expression?.GetNextOccurrence(DateTimeOffset.UtcNow, timeZoneInfo);
        }

        /// <summary>
        /// GetNextOccurrence
        /// </summary>
        /// <param name="expression">cron expression</param>
        /// <param name="period">next period</param>
        /// <returns>next occurrence times</returns>
        public static IEnumerable<DateTimeOffset> GetNextOccurrences(this CronExpression expression, TimeSpan period)
        {
            Guard.NotNull(expression, nameof(expression));
            var utcNow = DateTime.UtcNow;
            return expression.GetOccurrences(utcNow, utcNow.Add(period), TimeZoneInfo.Utc);
        }

        /// <summary>
        /// GetNextOccurrence
        /// </summary>
        /// <param name="expression">cron expression</param>
        /// <param name="period">next period</param>
        /// <param name="timeZoneInfo">timeZoneInfo</param>
        /// <returns>next occurrence times</returns>
        public static IEnumerable<DateTimeOffset> GetNextOccurrences(this CronExpression expression, TimeSpan period, TimeZoneInfo timeZoneInfo)
        {
            var utcNow = DateTime.UtcNow;
            return expression.GetOccurrences(utcNow, utcNow.Add(period), timeZoneInfo);
        }
    }
}
