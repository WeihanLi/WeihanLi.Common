using System;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Logging;

public interface ILogHelperLoggingEnricher : IEnricher<LogHelperLoggingEvent>
{
}

internal sealed class PropertyLoggingEnricher : PropertyEnricher<LogHelperLoggingEvent>, ILogHelperLoggingEnricher
{
    public PropertyLoggingEnricher(string propertyName, object propertyValue, bool overwrite = false) : base(propertyName, propertyValue, overwrite)
    {
    }

    public PropertyLoggingEnricher(string propertyName, Func<LogHelperLoggingEvent, object> propertyValueFactory, bool overwrite = false) : base(propertyName, propertyValueFactory, overwrite)
    {
    }

    public PropertyLoggingEnricher(string propertyName, Func<LogHelperLoggingEvent, object> propertyValueFactory, Func<LogHelperLoggingEvent, bool> propertyPredict, bool overwrite = false) : base(propertyName, propertyValueFactory, propertyPredict, overwrite)
    {
    }

    protected override Action<LogHelperLoggingEvent, string, Func<LogHelperLoggingEvent, object>, bool> EnrichAction =>
        (invocation, propertyName, propertyValueFactory, overwrite) =>
            invocation.AddProperty(propertyName, propertyValueFactory, overwrite);
}
