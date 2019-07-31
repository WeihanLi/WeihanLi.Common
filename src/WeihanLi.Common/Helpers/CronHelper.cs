using System;
using System.Collections.Generic;
using WeihanLi.Common.Helpers.Cron;

namespace WeihanLi.Common.Helpers
{
    /// <summary>
    /// CronExpression Helper
    /// </summary>
    public static class CronHelper
    {
        /// <summary>
        /// GetNextOccurrence
        /// </summary>
        /// <param name="cron">cron</param>
        /// <returns>next occurrence time</returns>
        public static DateTimeOffset? GetNextOccurrence(string cron)
        {
            var expression = CronExpression.Parse(cron, CronFormat.IncludeSeconds);
            return expression.GetNextOccurrence(DateTimeOffset.UtcNow, TimeZoneInfo.Utc);
        }

        /// <summary>
        /// GetNextOccurrence
        /// </summary>
        /// <param name="cron">cron expression</param>
        /// <param name="period">next period</param>
        /// <returns>next occurrence times</returns>
        public static IEnumerable<DateTimeOffset> GetNextOccurrences(string cron, TimeSpan period)
        {
            var expression = CronExpression.Parse(cron, CronFormat.IncludeSeconds);
            var fromUtc = DateTime.UtcNow;
            return expression.GetOccurrences(fromUtc, fromUtc.Add(period), TimeZoneInfo.Utc);
        }
    }
}
