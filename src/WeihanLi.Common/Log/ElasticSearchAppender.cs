using System;
using System.Net.Http;
using System.Text;
using log4net.Appender;
using log4net.Core;
using Newtonsoft.Json;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Log
{
    public class ElasticSearchAppender : BufferingAppenderSkeleton
    {
        private const int DefaultOnCloseTimeout = 30000;

        public string Index { get; set; }

        public string Type { get; set; } = "logEvent";

        private readonly HttpClient _httpClient;

        public ElasticSearchAppender()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/json");
        }

        public string ConnectionString { get; set; }

        public string ApplicationName { get; set; }

        protected override void SendBuffer(LoggingEvent[] events)
        {
            var sb = new StringBuilder(4096);
            foreach (var le in events)
            {
                try
                {
                    var json = JsonConvert.SerializeObject(le);
                    sb.AppendLine("{\"index\": {} }");
                    sb.AppendLine(json);
                }
                catch (Exception ex)
                {
                    ErrorHandler.Error(ex.Message, ex);
                }
            }

            var uri = GetUri();

            _httpClient.PostAsync(uri, new StringContent(sb.ToString())).ContinueWith(_ => _.Result.Dispose());
        }

        private Uri GetUri()
        {
            var uri = new UriBuilder(ConnectionString)
            {
                Path = $"/{ApplicationName ?? ApplicationHelper.ApplicationName}/{Index}-{DateTime.Today}/{Type}/_bulk"
            };

            return uri.Uri;
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
