using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Helpers.PeriodBatching;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Logging
{
    internal class PeriodBatchingLoggingService : PeriodicBatching<LogHelperLoggingEvent>
    {
        private readonly LogHelperFactory _logHelperFactory;
        private readonly SemaphoreSlim _semaphore;

        public PeriodBatchingLoggingService(int batchSizeLimit, TimeSpan period, LogHelperFactory logHelperFactory) : base(batchSizeLimit, period)
        {
            _logHelperFactory = logHelperFactory;
            _semaphore = new SemaphoreSlim(_logHelperFactory._logHelperProviders.Count, Environment.ProcessorCount * 8);
        }

        protected override async Task EmitBatchAsync(IEnumerable<LogHelperLoggingEvent> events)
        {
            if (_logHelperFactory._logHelperProviders.Count > 0)
            {
                var tasks = events.Select(loggingEvent => _logHelperFactory._logHelperProviders.Select(
                    async logHelperProvider =>
                    {
                        if (_logHelperFactory._logFilters.All(x => x.Invoke(logHelperProvider.Key,
                            loggingEvent.CategoryName, loggingEvent.LogLevel, loggingEvent.Exception)))
                        {
                            try
                            {
                                await _semaphore.WaitAsync();
                                await logHelperProvider.Value.Log(loggingEvent);
                            }
                            catch (Exception e)
                            {
                                InvokeHelper.OnInvokeException?.Invoke(e);
                            }
                            finally
                            {
                                _semaphore.Release();
                            }
                        }
                    }));

                await tasks.SelectMany(t => t).WhenAll();
            }
        }
    }
}
