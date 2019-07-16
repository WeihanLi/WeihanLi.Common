using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using log4net.Appender;
using log4net.Core;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Logging.Log4Net
{
    public class ElasticSearchAppender : BufferingAppenderSkeleton
    {
        private const int DefaultOnCloseTimeout = 30000;

        private static readonly HttpClient _httpClient;

        static ElasticSearchAppender()
        {
            _httpClient = new HttpClient() { Timeout = TimeSpan.FromMilliseconds(DefaultOnCloseTimeout) };
        }

        /// <summary>
        /// ElasticSearchUrl
        /// </summary>
        public string ElasticSearchUrl { get; set; }

        public string ApplicationName { get; set; }

        public string IndexFormat { get; set; } = "logstash-{applicationName}-{rollingDate}";

        public string Type { get; set; } = "logEvent";

        protected override void SendBuffer(LoggingEvent[] events)
        {
            if (events == null || events.Length == 0)
                return;

            var sb = new StringBuilder(4096);
            foreach (var le in events)
            {
                try
                {
                    sb.AppendLine("{ \"index\" : {} }");
                    var json = new
                    {
                        le.LoggerName,
                        Level = le.Level.Name,
                        TimeStamp = le.TimeStampUtc,
                        Message = le.RenderedMessage,
                        Exception = le.GetExceptionString(),
                        Properties = GetLoggingEventProperties(le).ToDictionary(),
                    }.ToJson(new Newtonsoft.Json.JsonSerializerSettings()
                    {
                        NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                        DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc,
                        ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
                        MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Ignore,
                    });
                    sb.AppendLine(json);
                }
                catch (Exception ex)
                {
                    ErrorHandler.Error(ex.Message, ex);
                    InvokeHelper.OnInvokeException?.Invoke(ex);
                }
            }

            var url = $"{ElasticSearchUrl}/{IndexFormat.Replace("{applicationName}", ApplicationName.GetValueOrDefault(ApplicationHelper.ApplicationName).ToLower()).Replace("{rollingDate}", DateTime.UtcNow.ToString("yyyyMMdd"))}/{Type}/_bulk";
            try
            {
                _httpClient.PostAsync(url, new StringContent(sb.ToString(), Encoding.UTF8, "application/json"))
                    .ContinueWith(_ => _.Result.Dispose()).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                ErrorHandler.Error(ex.Message, ex);
                InvokeHelper.OnInvokeException?.Invoke(ex);
            }
        }

        private static bool IsValidLog4NetPropertyValue(string value)
        {
            if (value.IsNullOrWhiteSpace())
            {
                return false;
            }
            value = value.Trim();
            if ("?" == value)
            {
                return false;
            }
            if ("NOT AVAILABLE" == value)
            {
                return false;
            }

            return true;
        }

        private static IEnumerable<KeyValuePair<string, object>> GetLoggingEventProperties(LoggingEvent loggingEvent)
        {
            yield return new KeyValuePair<string, object>("Host", Environment.MachineName);
            if (IsValidLog4NetPropertyValue(loggingEvent.ThreadName))
            {
                yield return new KeyValuePair<string, object>(nameof(loggingEvent.ThreadName), loggingEvent.ThreadName);
            }
            else
            {
                yield return new KeyValuePair<string, object>(nameof(loggingEvent.ThreadName), Thread.CurrentThread.Name);
            }
            if (IsValidLog4NetPropertyValue(loggingEvent.Identity))
            {
                yield return new KeyValuePair<string, object>(nameof(loggingEvent.Identity), loggingEvent.Identity);
            }
            if (IsValidLog4NetPropertyValue(loggingEvent.UserName))
            {
                yield return new KeyValuePair<string, object>(nameof(loggingEvent.UserName), loggingEvent.UserName);
            }
            if (IsValidLog4NetPropertyValue(loggingEvent.Domain))
            {
                yield return new KeyValuePair<string, object>(nameof(loggingEvent.Domain), loggingEvent.Domain);
            }

            var locInfo = loggingEvent.LocationInformation;
            if (locInfo != null)
            {
                if (IsValidLog4NetPropertyValue(locInfo.ClassName))
                {
                    yield return new KeyValuePair<string, object>(nameof(locInfo.ClassName), locInfo.ClassName);
                }

                if (IsValidLog4NetPropertyValue(locInfo.FileName))
                {
                    yield return new KeyValuePair<string, object>(nameof(locInfo.FileName), locInfo.FileName);
                }

                if (IsValidLog4NetPropertyValue(locInfo.LineNumber) && int.TryParse(locInfo.LineNumber, out var lineNumber) && lineNumber != 0)
                {
                    yield return new KeyValuePair<string, object>(nameof(locInfo.LineNumber), lineNumber);
                }

                if (IsValidLog4NetPropertyValue(locInfo.MethodName))
                {
                    yield return new KeyValuePair<string, object>(nameof(locInfo.MethodName), locInfo.MethodName);
                }
            }

            var properties = loggingEvent.GetProperties();
            if (properties == null)
            {
                yield break;
            }

            foreach (var key in properties.GetKeys())
            {
                if (!string.IsNullOrWhiteSpace(key)
                    && !key.StartsWith("log4net:", StringComparison.OrdinalIgnoreCase))
                {
                    var value = properties[key];
                    if (value != null
                        && (!(value is string stringValue) || IsValidLog4NetPropertyValue(stringValue)))
                    {
                        yield return new KeyValuePair<string, object>(key, value);
                    }
                }
            }
        }
    }
}
