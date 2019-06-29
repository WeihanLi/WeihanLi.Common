using System;
using System.Net.Http;
using System.Text;
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
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        }

        /// <summary>
        /// ElasticSearchUrl
        /// </summary>
        public string ElasticSearchUrl { get; set; }

        public string ApplicationName { get; set; }

        public string IndexFormat { get; set; } = "logstash-{applicationName}";

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
                        le.ThreadName,
                        Level = le.Level.Name,
                        le.LoggerName,
                        Exception = le.GetExceptionString(),
                        le.Properties,
                        TimeStamp = le.TimeStampUtc,
                        Message = le.RenderedMessage,
                        le.Domain,
                        le.Identity,
                        le.UserName,
                        le.LocationInformation
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
                }
            }
            sb.Remove(sb.Length - 1, 1);

            var url = $"{ElasticSearchUrl}/{IndexFormat.Replace("{applicationName}", ApplicationName.GetValueOrDefault(ApplicationHelper.ApplicationName).ToLower())}-{DateTime.UtcNow:yyyyMMdd}/{Type}/_bulk";
            _httpClient.PostAsync(url, new StringContent(sb.ToString()))
                .ContinueWith(_ => _.Result.Dispose()).ConfigureAwait(false);
        }
    }
}
