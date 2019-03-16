using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace WeihanLi.Common.Logging
{
    public interface ILogHelperFactory
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
    }

    public class NullLogHelperFactory : ILogHelperFactory
    {
        public static readonly ILogHelperFactory Instance = new NullLogHelperFactory();

        private NullLogHelperFactory()
        {
        }

        public bool AddProvider(ILogHelperProvider provider) => false;

        public ILogHelperLogger CreateLogger(string categoryName) => NullLogHelperLogger.Instance;
    }

    internal class LogHelperFactory : ILogHelperFactory
    {
        private readonly ConcurrentDictionary<Type, ILogHelperProvider> _logHelperProviders = new ConcurrentDictionary<Type, ILogHelperProvider>();

        private readonly ConcurrentDictionary<string, ILogHelperLogger> _logHelpers = new ConcurrentDictionary<string, ILogHelperLogger>();

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

        public ILogHelperLogger CreateLogger(string categoryName) => _logHelpers.GetOrAdd(categoryName, _ => new LogHelper(_logHelperProviders.Values, _));

        public bool AddProvider(ILogHelperProvider provider) => _logHelperProviders.TryAdd(provider.GetType(), provider);
    }
}
