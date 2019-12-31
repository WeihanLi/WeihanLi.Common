using System;
using System.Collections.Generic;

namespace WeihanLi.Common.Logging
{
    /// <summary>
    /// LogHelperExtensions
    /// </summary>
    public static class LogHelperExtensions
    {
        public static void Log(this ILogHelperLogger logger, LogHelperLogLevel loggerLevel, string msg) => logger.Log(loggerLevel, null, msg);

        #region Info

        public static void Info(this ILogHelperLogger logger, string msg, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                logger.Log(LogHelperLogLevel.Info, msg);
            }
            else
            {
                logger.Log(LogHelperLogLevel.Info, null, msg, parameters);
            }
        }

        public static void Info(this ILogHelperLogger logger, Exception ex, string msg) => logger.Log(LogHelperLogLevel.Info, ex, msg);

        public static void Info(this ILogHelperLogger logger, Exception ex) => logger.Log(LogHelperLogLevel.Info, ex, ex?.Message);

        #endregion Info

        #region Trace

        public static void Trace(this ILogHelperLogger logger, string msg, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                logger.Log(LogHelperLogLevel.Trace, msg);
            }
            else
            {
                logger.Log(LogHelperLogLevel.Trace, null, msg, parameters);
            }
        }

        public static void Trace(this ILogHelperLogger logger, Exception ex, string msg) => logger.Log(LogHelperLogLevel.Trace, ex, msg);

        public static void Trace(this ILogHelperLogger logger, Exception ex) => logger.Log(LogHelperLogLevel.Trace, ex, ex?.Message);

        #endregion Trace

        #region Debug

        public static void Debug(this ILogHelperLogger logger, string msg, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                logger.Log(LogHelperLogLevel.Debug, msg);
            }
            else
            {
                logger.Log(LogHelperLogLevel.Debug, null, msg, parameters);
            }
        }

        public static void Debug(this ILogHelperLogger logger, Exception ex, string msg) => logger.Log(LogHelperLogLevel.Debug, ex, msg);

        public static void Debug(this ILogHelperLogger logger, Exception ex) => logger.Log(LogHelperLogLevel.Debug, ex, ex?.Message);

        #endregion Debug

        #region Warn

        public static void Warn(this ILogHelperLogger logger, string msg, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                logger.Log(LogHelperLogLevel.Warn, null, msg);
            }
            else
            {
                logger.Log(LogHelperLogLevel.Warn, null, msg, parameters);
            }
        }

        public static void Warn(this ILogHelperLogger logger, Exception ex, string msg) => logger.Log(LogHelperLogLevel.Warn, ex, msg);

        public static void Warn(this ILogHelperLogger logger, Exception ex) => logger.Log(LogHelperLogLevel.Warn, ex, ex?.Message);

        #endregion Warn

        #region Error

        public static void Error(this ILogHelperLogger logger, string msg, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                logger.Log(LogHelperLogLevel.Error, null, msg);
            }
            else
            {
                logger.Log(LogHelperLogLevel.Error, null, msg, parameters);
            }
        }

        public static void Error(this ILogHelperLogger logger, Exception ex, string msg) => logger.Log(LogHelperLogLevel.Error, ex, msg);

        public static void Error(this ILogHelperLogger logger, Exception ex) => logger.Log(LogHelperLogLevel.Error, ex, ex?.Message);

        #endregion Error

        #region Fatal

        public static void Fatal(this ILogHelperLogger logger, string msg, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                logger.Log(LogHelperLogLevel.Fatal, null, msg);
            }
            else
            {
                logger.Log(LogHelperLogLevel.Fatal, null, msg, parameters);
            }
        }

        public static void Fatal(this ILogHelperLogger logger, Exception ex, string msg) => logger.Log(LogHelperLogLevel.Fatal, ex, msg);

        public static void Fatal(this ILogHelperLogger logger, Exception ex) => logger.Log(LogHelperLogLevel.Fatal, ex, ex?.Message);

        #endregion Fatal

        #region ILogHelperFactory

        public static ILogHelperLogger GetLogger<T>(this ILogHelperFactory logHelperFactory) =>
            GetLogger(logHelperFactory, typeof(T));

        public static ILogHelperLogger GetLogger(this ILogHelperFactory logHelperFactory, Type type)
        {
            return logHelperFactory.CreateLogger(type.FullName);
        }

        #endregion ILogHelperFactory

        #region ILogHelperLoggingBuilder

        public static ILogHelperLoggingBuilder WithMinimumLevel(this ILogHelperLoggingBuilder loggingBuilder, LogHelperLogLevel logLevel)
        {
            return loggingBuilder.WithFilter(level => level >= logLevel);
        }

        public static ILogHelperLoggingBuilder WithFilter(this ILogHelperLoggingBuilder loggingBuilder, Func<LogHelperLogLevel, bool> filterFunc)
        {
            loggingBuilder.AddFilter((type, categoryName, logLevel, exception) => filterFunc.Invoke(logLevel));
            return loggingBuilder;
        }

        public static ILogHelperLoggingBuilder WithFilter(this ILogHelperLoggingBuilder loggingBuilder, Func<string, LogHelperLogLevel, bool> filterFunc)
        {
            loggingBuilder.AddFilter((type, categoryName, logLevel, exception) => filterFunc.Invoke(categoryName, logLevel));
            return loggingBuilder;
        }

        public static ILogHelperLoggingBuilder WithFilter(this ILogHelperLoggingBuilder loggingBuilder, Func<Type, string, LogHelperLogLevel, bool> filterFunc)
        {
            loggingBuilder.AddFilter((type, categoryName, logLevel, exception) => filterFunc.Invoke(type, categoryName, logLevel));
            return loggingBuilder;
        }

        public static ILogHelperLoggingBuilder WithFilter(this ILogHelperLoggingBuilder loggingBuilder, Func<Type, string, LogHelperLogLevel, Exception, bool> filterFunc)
        {
            loggingBuilder.AddFilter(filterFunc);
            return loggingBuilder;
        }

        public static ILogHelperLoggingBuilder WithProvider(this ILogHelperLoggingBuilder loggingBuilder, ILogHelperProvider logHelperProvider)
        {
            loggingBuilder.AddProvider(logHelperProvider);
            return loggingBuilder;
        }

        public static ILogHelperLoggingBuilder WithEnricher<TEnricher>(this ILogHelperLoggingBuilder loggingBuilder,
            TEnricher enricher) where TEnricher : ILogHelperLoggingEnricher
        {
            loggingBuilder.AddEnricher(enricher);
            return loggingBuilder;
        }

        public static ILogHelperLoggingBuilder WithEnricher<TEnricher>(this ILogHelperLoggingBuilder loggingBuilder) where TEnricher : ILogHelperLoggingEnricher, new()
        {
            loggingBuilder.AddEnricher(new TEnricher());
            return loggingBuilder;
        }

        public static ILogHelperLoggingBuilder EnrichWithProperty(this ILogHelperLoggingBuilder loggingBuilder, string propertyName, object value, bool overwrite = false)
        {
            loggingBuilder.AddEnricher(new PropertyLoggingEnricher(propertyName, value, overwrite));
            return loggingBuilder;
        }

        public static ILogHelperLoggingBuilder EnrichWithProperty(this ILogHelperLoggingBuilder loggingBuilder, string propertyName, Func<LogHelperLoggingEvent> valueFactory, bool overwrite = false)
        {
            loggingBuilder.AddEnricher(new PropertyLoggingEnricher(propertyName, valueFactory, overwrite));
            return loggingBuilder;
        }

        public static ILogHelperLoggingBuilder EnrichWithProperty(this ILogHelperLoggingBuilder loggingBuilder, string propertyName, object value, Func<LogHelperLoggingEvent, bool> predict, bool overwrite = false)
        {
            loggingBuilder.AddEnricher(new PropertyLoggingEnricher(propertyName, e => value, predict, overwrite));
            return loggingBuilder;
        }

        public static ILogHelperLoggingBuilder EnrichWithProperty(this ILogHelperLoggingBuilder loggingBuilder, string propertyName, Func<LogHelperLoggingEvent, object> valueFactory, Func<LogHelperLoggingEvent, bool> predict, bool overwrite = false)
        {
            loggingBuilder.AddEnricher(new PropertyLoggingEnricher(propertyName, valueFactory, predict, overwrite));
            return loggingBuilder;
        }

        #endregion ILogHelperLoggingBuilder

        #region LoggingEnricher

        public static void AddProperty(this LogHelperLoggingEvent loggingEvent, string propertyName,
            object propertyValue, bool overwrite = false)
        {
            if (null == loggingEvent)
            {
                throw new ArgumentNullException(nameof(loggingEvent));
            }

            if (loggingEvent.Properties == null)
            {
                loggingEvent.Properties = new Dictionary<string, object>();
            }
            if (loggingEvent.Properties.ContainsKey(propertyName) && overwrite == false)
            {
                return;
            }

            loggingEvent.Properties[propertyName] = propertyValue;
        }

        public static void AddProperty(this LogHelperLoggingEvent loggingEvent, string propertyName,
            Func<LogHelperLoggingEvent, object> propertyValueFactory, bool overwrite = false)
        {
            if (null == loggingEvent)
            {
                throw new ArgumentNullException(nameof(loggingEvent));
            }

            if (loggingEvent.Properties == null)
            {
                loggingEvent.Properties = new Dictionary<string, object>();
            }

            if (loggingEvent.Properties.ContainsKey(propertyName) && overwrite == false)
            {
                return;
            }

            loggingEvent.Properties[propertyName] = propertyValueFactory?.Invoke(loggingEvent);
        }

        #endregion LoggingEnricher
    }
}
