using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace WeihanLi.Common.Log
{
    public interface ILogHelperFactory
    {
        /// <summary>
        /// Creates a new ILogHelper instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        ILogHelper CreateLogHelper(string categoryName);

        /// <summary>
        /// Adds an ILogHelperProvider to the logging system.
        /// </summary>
        /// <param name="provider">The ILogHelperProvider.</param>
        bool AddProvider(ILogHelperProvider provider);
    }

    internal class LogHelperFactory : ILogHelperFactory
    {
        private readonly ConcurrentDictionary<Type, ILogHelperProvider> _logHelperProviders = new ConcurrentDictionary<Type, ILogHelperProvider>();

        private readonly ConcurrentDictionary<string, LogHelper> _logHelpers = new ConcurrentDictionary<string, LogHelper>();

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

        public ILogHelper CreateLogHelper(string categoryName) => _logHelpers.GetOrAdd(categoryName, _ => new LogHelper(_logHelperProviders.Values, _));

        public bool AddProvider(ILogHelperProvider provider) => _logHelperProviders.TryAdd(provider.GetType(), provider);
    }
}
