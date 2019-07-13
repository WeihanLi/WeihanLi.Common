using System;
using System.Collections.Generic;
using System.Linq;

#if NETSTANDARD2_0

using Microsoft.Extensions.Logging;

#endif

using WeihanLi.Common.Logging;

namespace WeihanLi.Common.Helpers
{
    /// <summary>
    /// LogHelper
    /// Logging
    /// </summary>
    public static class LogHelper
    {
        private static readonly Lazy<ILogHelperFactory> _logFactory = new Lazy<ILogHelperFactory>(() => new LogHelperFactory());

        public static ILogHelperFactory LogFactory => _logFactory.Value;

        public static ILogHelperLogger GetLogger<T>() => GetLogger(typeof(T));

        public static ILogHelperLogger GetLogger(Type type) => GetLogger(type.FullName);

        public static ILogHelperLogger GetLogger(string categoryName)
        {
            return _logFactory.Value.CreateLogger(categoryName);
        }

        public static bool AddLogProvider(ILogHelperProvider logHelperProvider)
        {
            return _logFactory.Value.AddProvider(logHelperProvider);
        }

        public static int AddLogProvider(ICollection<ILogHelperProvider> logProviders)
        {
            if (logProviders != null && logProviders.Count > 0)
            {
                var results = new bool[logProviders.Count];
                var idx = 0;
                foreach (var provider in logProviders)
                {
                    if (provider != null)
                    {
                        results[idx] = _logFactory.Value.AddProvider(provider);
                    }
                    idx++;
                }
                return results.Count(_ => _);
            }

            return 0;
        }
    }
}
