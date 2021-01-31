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
        bool AddFilter(Func<Type, LogHelperLoggingEvent, bool> filterFunc);

        /// <summary>
        /// Build for LogFactory
        /// </summary>
        /// <returns></returns>
        ILogHelperFactory Build();
    }

    internal class LogHelperLoggingBuilder : ILogHelperLoggingBuilder
    {
        internal readonly Dictionary<Type, ILogHelperProvider> _logHelperProviders = new();
        internal readonly List<ILogHelperLoggingEnricher> _logHelperEnrichers = new();
        internal readonly List<Func<Type, LogHelperLoggingEvent, bool>> _logFilters = new();

        public bool AddProvider(ILogHelperProvider provider)
        {
            Guard.NotNull(provider, nameof(provider));

            return _logHelperProviders.AddIfNotContainsKey(provider.GetType(), provider);
        }

        public bool AddEnricher(ILogHelperLoggingEnricher enricher)
        {
            Guard.NotNull(enricher, nameof(enricher));
            _logHelperEnrichers.Add(enricher);
            return true;
        }

        public bool AddFilter(Func<Type, LogHelperLoggingEvent, bool> filterFunc)
        {
            Guard.NotNull(filterFunc, nameof(filterFunc));

            _logFilters.Add(filterFunc);
            return true;
        }

        public ILogHelperFactory Build() => new LogHelperFactory(_logHelperProviders, _logHelperEnrichers, _logFilters);
    }
}
