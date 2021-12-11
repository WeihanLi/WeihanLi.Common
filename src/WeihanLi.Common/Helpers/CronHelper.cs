using System;
using System.Collections.Generic;
using WeihanLi.Common.Helpers.Cron;

namespace WeihanLi.Common.Helpers;

/// <summary>
/// CronExpression Helper
/// Cron Expression : http://www.quartz-scheduler.org/documentation/quartz-2.3.0/tutorials/crontrigger.html
/// </summary>
public static class CronHelper
{
    public static readonly string Yearly = CronExpression.Yearly.ToString();
    public static readonly string Weekly = CronExpression.Weekly.ToString();
    public static readonly string Monthly = CronExpression.Monthly.ToString();
    public static readonly string Daily = CronExpression.Daily.ToString();
    public static readonly string Hourly = CronExpression.Hourly.ToString();
    public static readonly string Minutely = CronExpression.Minutely.ToString();
    public static readonly string Secondly = CronExpression.Secondly.ToString();

    /// <summary>
    /// GetNextOccurrence
    /// </summary>
    /// <param name="cron">cron</param>
    /// <returns>next occurrence time</returns>
    public static DateTimeOffset? GetNextOccurrence(string cron)
    {
        var expression = CronExpression.Parse(cron);
        return expression.GetNextOccurrence(DateTimeOffset.UtcNow, TimeZoneInfo.Utc);
    }

    /// <summary>
    /// GetNextOccurrence
    /// </summary>
    /// <param name="cron">cron</param>
    /// <param name="timeZoneInfo">timeZoneInfo</param>
    /// <returns>next occurrence time</returns>
    public static DateTimeOffset? GetNextOccurrence(string cron, TimeZoneInfo timeZoneInfo)
    {
        var expression = CronExpression.Parse(cron);
        return expression.GetNextOccurrence(DateTimeOffset.UtcNow, timeZoneInfo);
    }

    /// <summary>
    /// GetNextOccurrence
    /// </summary>
    /// <param name="cron">cron expression</param>
    /// <param name="period">next period</param>
    /// <returns>next occurrence times</returns>
    public static IEnumerable<DateTimeOffset> GetNextOccurrences(string cron, TimeSpan period)
    {
        var expression = CronExpression.Parse(cron);
        var fromUtc = DateTime.UtcNow;
        return expression.GetOccurrences(fromUtc, fromUtc.Add(period), TimeZoneInfo.Utc);
    }

    /// <summary>
    /// GetNextOccurrence
    /// </summary>
    /// <param name="cron">cron expression</param>
    /// <param name="period">next period</param>
    /// <param name="timeZoneInfo">timeZoneInfo</param>
    /// <returns>next occurrence times</returns>
    public static IEnumerable<DateTimeOffset> GetNextOccurrences(string cron, TimeSpan period, TimeZoneInfo timeZoneInfo)
    {
        var expression = CronExpression.Parse(cron);
        var fromUtc = DateTime.UtcNow;
        return expression.GetOccurrences(fromUtc, fromUtc.Add(period), timeZoneInfo);
    }
}
