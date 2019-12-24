using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace WeihanLi.Common.Logging
{
    public interface ILogHelperFactory : IDisposable
    {
        /// <summary>
        /// Creates a new ILogHelper instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        ILogHelperLogger CreateLogger(string categoryName);

        /// <summary>
        /// Adds an ILogHelperProvider to the logging system.
        /// </summary>
        /// <param name="provider">The ILogHelperProvider.</param>
        bool AddProvider(ILogHelperProvider provider);

        /// <summary>
        /// Add logs filter
        /// </summary>
        /// <param name="filterFunc">filterFunc, logProviderType/categoryName/Exception, whether to write log</param>
        bool AddFilter(Func<Type, string, LogHelperLevel, Exception, bool> filterFunc);

        ///// <summary>
        ///// config period batching
        ///// </summary>
        ///// <param name="period">period</param>
        ///// <param name="batchSize">batchSize</param>
        //void PeriodBatchingConfig(TimeSpan period, int batchSize);
    }

    internal class NullLogHelperFactory : ILogHelperFactory
    {
        public static readonly ILogHelperFactory Instance = new NullLogHelperFactory();

        public void Dispose()
        {
        }

        private NullLogHelperFactory()
        {
        }

        public bool AddProvider(ILogHelperProvider provider) => false;

        public ILogHelperLogger CreateLogger(string categoryName) => NullLogHelperLogger.Instance;

        public bool AddFilter(Func<Type, string, LogHelperLevel, Exception, bool> filterFunc)
        {
            return filterFunc != null;
        }

        public void PeriodBatchingConfig(TimeSpan period, int batchSize)
        {
        }
    }

    internal class LogHelperFactory : ILogHelperFactory
    {
        internal readonly ConcurrentDictionary<Type, ILogHelperProvider> _logHelperProviders = new ConcurrentDictionary<Type, ILogHelperProvider>();

        private readonly ConcurrentDictionary<string, ILogHelperLogger> _loggers = new ConcurrentDictionary<string, ILogHelperLogger>();

        internal readonly ConcurrentBag<Func<Type, string, LogHelperLevel, Exception, bool>> _logFilters = new ConcurrentBag<Func<Type, string, LogHelperLevel, Exception, bool>>();

        public LogHelperFactory() : this(Enumerable.Empty<ILogHelperProvider>())
        {
        }

        public LogHelperFactory(IEnumerable<ILogHelperProvider> logHelperProviders)
        {
            foreach (var provider in logHelperProviders)
            {
                if (provider != null)
                {
                    _logHelperProviders.TryAdd(provider.GetType(), provider);
                }
            }
        }

        public ILogHelperLogger CreateLogger(string categoryName) => _loggers.GetOrAdd(categoryName, _ => new LogHelper(this, _));

        public bool AddProvider(ILogHelperProvider provider) => _logHelperProviders.TryAdd(provider.GetType(), provider);

        public bool AddFilter(Func<Type, string, LogHelperLevel, Exception, bool> filterFunc)
        {
            if (null == filterFunc)
            {
                return false;
            }

            _logFilters.Add(filterFunc);
            return true;
        }

        //internal TimeSpan Period { get; private set; } = TimeSpan.FromSeconds(5);
        //internal int BatchSize { get; private set; } = 1000;
        //public void PeriodBatchingConfig(TimeSpan period, int batchSize)
        //{
        //    if (period > TimeSpan.Zero)
        //    {
        //        Period = period;
        //    }
        //    if (batchSize > 0)
        //    {
        //        BatchSize = batchSize;
        //    }
        //    else if (batchSize == 0)
        //    {
        //        BatchSize = 1;
        //    }
        //}

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

            _logHelperProviders.Clear();
        }
    }
}
