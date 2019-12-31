using System;
using System.Collections.Generic;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Logging
{
    public interface ILogHelperLoggingBuilder
    {
        /// <summary>
        /// Adds an ILogHelperProvider to the logging system.
        /// </summary>
        /// <param name="provider">The ILogHelperProvider.</param>
        bool AddProvider(ILogHelperProvider provider);

        /// <summary>
        /// add log enricher
        /// </summary>
        /// <param name="enricher">log enricher</param>
        /// <returns></returns>
        bool AddEnricher(ILogHelperLoggingEnricher enricher);

        /// <summary>
        /// Add logs filter
        /// </summary>
        /// <param name="filterFunc">filterFunc, logProviderType/categoryName/Exception, whether to write log</param>
        bool AddFilter(Func<Type, string, LogHelperLogLevel, Exception, bool> filterFunc);

        ///// <summary>
        ///// config period batching
        ///// </summary>
        ///// <param name="period">period</param>
        ///// <param name="batchSize">batchSize</param>
        //void PeriodBatchingConfig(TimeSpan period, int batchSize);

        /// <summary>
        /// Build for LogFactory
        /// </summary>
        /// <returns></returns>
        ILogHelperFactory Build();
    }

    public class LogHelperLoggingBuilder : ILogHelperLoggingBuilder
    {
        internal readonly Dictionary<Type, ILogHelperProvider> _logHelperProviders = new Dictionary<Type, ILogHelperProvider>();
        internal readonly List<ILogHelperLoggingEnricher> _logHelperEnrichers = new List<ILogHelperLoggingEnricher>();
        internal readonly List<Func<Type, string, LogHelperLogLevel, Exception, bool>> _logFilters = new List<Func<Type, string, LogHelperLogLevel, Exception, bool>>();

        public bool AddProvider(ILogHelperProvider provider)
        {
            if (null == provider)
                return false;

            return _logHelperProviders.AddIfNotContainsKey(provider.GetType(), provider);
        }

        public bool AddEnricher(ILogHelperLoggingEnricher enricher)
        {
            if (null != enricher)
            {
                _logHelperEnrichers.Add(enricher);
                return true;
            }
            return false;
        }

        public bool AddFilter(Func<Type, string, LogHelperLogLevel, Exception, bool> filterFunc)
        {
            if (null == filterFunc)
            {
                return false;
            }

            _logFilters.Add(filterFunc);
            return true;
        }

        public ILogHelperFactory Build() => new LogHelperFactory(_logHelperProviders, _logHelperEnrichers, _logFilters);
    }
}
