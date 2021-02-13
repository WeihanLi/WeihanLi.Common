using System;
using System.Collections.Generic;

namespace WeihanLi.Common.Logging
{
    public class LogHelperLoggingEvent
    {
        public string CategoryName { get; set; } = null!;

        public DateTimeOffset DateTime { get; set; }

        public string MessageTemplate { get; set; } = null!;

        public string Message { get; set; } = null!;

        public Exception? Exception { get; set; }

        public LogHelperLogLevel LogLevel { get; set; }

        public Dictionary<string, object?>? Properties { get; set; }

        public LogHelperLoggingEvent Copy()
        {
            var newEvent = (LogHelperLoggingEvent)MemberwiseClone();
            if (Properties != null)
            {
                newEvent.Properties = new Dictionary<string, object?>();
                foreach (var property in Properties)
                {
                    newEvent.Properties[property.Key] = property.Value;
                }
            }
            return newEvent;
        }
    }
}
