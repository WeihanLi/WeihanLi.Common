using System;
using System.Linq;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace DotNetCoreSample
{
    public static class CronHelperTest
    {
        public static void MainTest()
        {
            // http://www.quartz-scheduler.org/documentation/quartz-2.3.0/tutorials/crontrigger.html
            var nextTickUtc = CronHelper.GetNextOccurrence("0 15 10 * * ?");
            var nextTickLocal = CronHelper.GetNextOccurrence("0 15 10 * * ?", TimeZoneInfo.Local);

            Console.WriteLine($"@utc next tick: {nextTickUtc.GetValueOrDefault().DateTime.ToStandardTimeString()}  {Environment.NewLine} local next tick:{nextTickLocal.GetValueOrDefault().DateTime.ToStandardTimeString()}");

            var nextTicks = CronHelper.GetNextOccurrences("0 15 10-20 * * ?", TimeSpan.FromHours(6), TimeZoneInfo.Local).Take(5).ToArray();
            foreach (var tick in nextTicks)
            {
                Console.WriteLine(tick.DateTime.ToStandardTimeString());
            }
        }
    }
}
