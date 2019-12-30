using System;
using System.Collections.Generic;

namespace WeihanLi.Common.Logging
{
    public class LogHelperLoggingEvent : ICloneable
    {
        public string CategoryName { get; set; }

        public DateTimeOffset DateTime { get; set; }

        public string MessageTemplate { get; set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }

        public LogHelperLevel LogLevel { get; set; }

        public Dictionary<string, object> Properties { get; set; }

        public LogHelperLoggingEvent Copy => (LogHelperLoggingEvent)Clone();

        public object Clone()
        {
            var newEvent = (LogHelperLoggingEvent)MemberwiseClone();
            if (Properties != null)
            {
                newEvent.Properties = new Dictionary<string, object>();
                foreach (var property in Properties)
                {
                    newEvent.Properties[property.Key] = property.Value;
                }
            }
            return newEvent;
        }
    }
}
