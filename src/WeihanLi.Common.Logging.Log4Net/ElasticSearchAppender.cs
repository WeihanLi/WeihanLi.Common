using System;
using System.Net.Http;
using System.Text;
using log4net.Appender;
using log4net.Core;
using Newtonsoft.Json;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Logging.Log4Net
{
    public class ElasticSearchAppender : BufferingAppenderSkeleton
    {
        private const int DefaultOnCloseTimeout = 30000;

        public string Type { get; set; } = "logEvent";

        private readonly HttpClient _httpClient;

        public ElasticSearchAppender()
        {
            _httpClient = new HttpClient() { Timeout = TimeSpan.FromMilliseconds(DefaultOnCloseTimeout) };
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        }

        public string ElasticSearchUrl { get; set; }

        public string ApplicationName { get; set; }

        public string IndexFormat { get; set; } = "logstash-{applicationName}";

        protected override void SendBuffer(LoggingEvent[] events)
        {
            var sb = new StringBuilder(4096);
            foreach (var le in events)
            {
                try
                {
                    sb.AppendLine("{ \"index\" : {} }");
                    var json = JsonConvert.SerializeObject(le);
                    sb.AppendLine(json);
                }
                catch (Exception ex)
                {
                    ErrorHandler.Error(ex.Message, ex);
                }
            }
            sb.Remove(sb.Length - 1, 1);

            var url = $"{ElasticSearchUrl}/{IndexFormat.Replace("{applicationName}", (ApplicationName ?? ApplicationHelper.ApplicationName).ToLower())}-{DateTime.Today:yyyyMMdd}/{Type}/_bulk";
            _httpClient.PostAsync(url, new StringContent(sb.ToString()))
                .ContinueWith(_ => _.Result.Dispose()).ConfigureAwait(false);
        }

        #region Dispose

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                //释放托管资源，比如将对象设置为null
            }

            //释放非托管资源
            _httpClient?.Dispose();

            _disposed = true;
        }

        ~ElasticSearchAppender()
        {
            Dispose(false);
        }

        #endregion Dispose
    }
}
