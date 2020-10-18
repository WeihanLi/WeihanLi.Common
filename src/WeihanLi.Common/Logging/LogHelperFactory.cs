using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace WeihanLi.Common.Logging
{
    public interface ILogHelperFactory : IDisposable
    {
        /// <summary>
        /// Creates a new ILogHelper instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        ILogHelperLogger CreateLogger(string categoryName);
    }

    internal sealed class NullLogHelperFactory : ILogHelperFactory
    {
        public static readonly ILogHelperFactory Instance = new NullLogHelperFactory();

        public void Dispose()
        {
            // nothing to dispose
        }

        private NullLogHelperFactory()
        {
        }

        public ILogHelperLogger CreateLogger(string categoryName) => NullLogHelperLogger.Instance;
    }

    internal sealed class LogHelperFactory : ILogHelperFactory
    {
        internal readonly IReadOnlyDictionary<Type, ILogHelperProvider> _logHelperProviders;
        internal readonly IReadOnlyCollection<ILogHelperLoggingEnricher> _logHelperEnrichers;
        internal readonly IReadOnlyCollection<Func<Type, LogHelperLoggingEvent, bool>> _logFilters;

        private readonly ConcurrentDictionary<string, ILogHelperLogger> _loggers = new ConcurrentDictionary<string, ILogHelperLogger>();

        public LogHelperFactory(IReadOnlyDictionary<Type, ILogHelperProvider> logHelperProviders,
            IReadOnlyCollection<ILogHelperLoggingEnricher> logHelperEnrichers,
            IReadOnlyCollection<Func<Type, LogHelperLoggingEvent, bool>> logFilters
            )
        {
            _logHelperProviders = logHelperProviders;
            _logHelperEnrichers = logHelperEnrichers;
            _logFilters = logFilters;
        }

        public ILogHelperLogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, _ => new LogHelper(this, _));

        public void Dispose()
        {
            if (_logHelperProviders.Count == 0)
                return;

            foreach (var provider in _logHelperProviders.Values)
            {
                if (provider is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }
    }
}
