using System;

namespace WeihanLi.Common.Logging
{
    public interface ILogHelperLoggingEnricher
    {
        void Enrich(LogHelperLoggingEvent loggingEvent);
    }

    internal class PropertyLoggingEnricher : ILogHelperLoggingEnricher
    {
        private readonly string _propertyName;
        private readonly Func<LogHelperLoggingEvent, object> _propertyValueFactory;
        private readonly bool _overwrite;
        private readonly Func<LogHelperLoggingEvent, bool> _logPropertyPredict;

        public PropertyLoggingEnricher(string propertyName, object propertyValue, bool overwrite = false) : this(propertyName, (loggingEvent) => propertyValue, overwrite)
        {
        }

        public PropertyLoggingEnricher(string propertyName, Func<LogHelperLoggingEvent, object> propertyValueFactory,
            bool overwrite = false) : this(propertyName, propertyValueFactory, null, overwrite)
        {
        }

        public PropertyLoggingEnricher(string propertyName, Func<LogHelperLoggingEvent, object> propertyValueFactory, Func<LogHelperLoggingEvent, bool> logPropertyPredict,
            bool overwrite = false)
        {
            _propertyName = propertyName;
            _propertyValueFactory = propertyValueFactory;
            _logPropertyPredict = logPropertyPredict;
            _overwrite = overwrite;
        }

        public void Enrich(LogHelperLoggingEvent loggingEvent)
        {
            if (_logPropertyPredict?.Invoke(loggingEvent) != false)
            {
                loggingEvent.AddProperty(_propertyName, _propertyValueFactory, _overwrite);
            }
        }
    }
}
