using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers
{
    public class HttpRequest
    {
        #region private fields

        private readonly HttpWebRequest _request;
        private byte[] _requestDataBytes;
        private readonly string _requestUrl;

        #endregion private fields

        #region ctor

        /// <summary>
        /// Create HttpRequest with GET Request Method
        /// </summary>
        /// <param name="requestUrl">requestUrl</param>
        public HttpRequest(string requestUrl) : this(requestUrl, HttpMethod.Get)
        {
        }

        /// <summary>
        /// Create HttpRequest with specific Request Method
        /// </summary>
        /// <param name="requestUrl">requestUrl</param>
        /// <param name="queryDictionary">queryDictionary</param>
        /// <param name="method">method</param>
        public HttpRequest(string requestUrl, IDictionary<string, string> queryDictionary, HttpMethod method)
        {
            _requestUrl = $"{requestUrl}{(requestUrl.Contains("?") ? "&" : "?")}{queryDictionary.ToQueryString()}";
            _request = WebRequest.CreateHttp(requestUrl);
            _request.UserAgent = HttpHelper.GetUserAgent();

            _request.Method = method.Method;
        }

        /// <summary>
        /// Create HttpRequest with specific Request Method
        /// </summary>
        /// <param name="requestUrl">requestUrl</param>
        /// <param name="method">request method</param>
        public HttpRequest(string requestUrl, HttpMethod method)
        {
            _requestUrl = requestUrl;
            _request = WebRequest.CreateHttp(requestUrl);
            _request.UserAgent = HttpHelper.GetUserAgent();

            _request.Method = method.Method;
        }

        #endregion ctor

        #region AddHeader

        public HttpRequest WithHeaders([NotNull]NameValueCollection customHeaders) => WithHeaders(customHeaders.ToDictionary());

        public HttpRequest WithHeaders([NotNull]IEnumerable<KeyValuePair<string, string>> customHeaders)
        {
            foreach (var header in customHeaders)
            {
                if (header.Key.EqualsIgnoreCase("REFERER"))
                {
                    _request.Referer = header.Value;
                    continue;
                }
                if (header.Key.EqualsIgnoreCase("User-Agent"))
                {
                    _request.UserAgent = header.Value;
                    continue;
                }
                if (header.Key.EqualsIgnoreCase("COOKIE"))
                {
                    var cookieCollection = new CookieCollection();
                    var host = new Uri(_requestUrl).Host;
                    header.Value.Split(';').ForEach(c => cookieCollection.Add(new Cookie(c.Split('=')[0].Trim(), c.Split('=')[1].Trim(), "/", host)));
                    WithCookie(cookieCollection);
                    continue;
                }
                _request.Headers[header.Key] = header.Value;
            }
            return this;
        }

        #endregion AddHeader

        #region private method

        private static bool IsNeedRequestStream(string requestMethod) => requestMethod.EqualsIgnoreCase("POST") || requestMethod.EqualsIgnoreCase("PUT");

        #endregion private method

        #region UserAgent

        public HttpRequest WithUserAgent(bool isMobile)
        {
            _request.UserAgent = HttpHelper.GetUserAgent(isMobile);
            return this;
        }

        public HttpRequest WithUserAgent(string userAgent)
        {
            _request.UserAgent = userAgent;
            return this;
        }

        #endregion UserAgent

        #region Referer

        public HttpRequest WithReferer(string referer)
        {
            _request.Referer = referer;
            return this;
        }

        #endregion Referer

        #region Proxy

        public HttpRequest WithProxy(string url)
        {
            _request.Proxy = new WebProxy(new Uri(url));
            return this;
        }

        public HttpRequest WithProxy(string url, string userName, string password)
        {
            _request.Proxy = new WebProxy(new Uri(url))
            {
                Credentials = new NetworkCredential(userName, password)
            };
            return this;
        }

        public HttpRequest WithProxy(WebProxy proxy)
        {
            _request.Proxy = proxy;
            return this;
        }

        #endregion Proxy

        #region Cookie

        public HttpRequest WithCookie(Cookie cookie)
        {
            if (null == _request.CookieContainer)
            {
                _request.CookieContainer = new CookieContainer();
            }
            _request.CookieContainer.Add(cookie);
            return this;
        }

        public HttpRequest WithCookie(string url, Cookie cookie)
        {
            if (null == _request.CookieContainer)
            {
                _request.CookieContainer = new CookieContainer();
            }
            _request.CookieContainer.Add(new Uri(url), cookie);
            return this;
        }

        public HttpRequest WithCookie(CookieCollection cookies)
        {
            if (null == _request.CookieContainer)
            {
                _request.CookieContainer = new CookieContainer();
            }
            _request.CookieContainer.Add(cookies);
            return this;
        }

        public HttpRequest WithCookie(string url, CookieCollection cookies)
        {
            if (null == _request.CookieContainer)
            {
                _request.CookieContainer = new CookieContainer();
            }
            _request.CookieContainer.Add(new Uri(url), cookies);
            return this;
        }

        #endregion Cookie

        #region Parameter

        public HttpRequest WithFormParameters([NotNull] NameValueCollection parameters) => WithFormParameters(parameters.ToDictionary());

        public HttpRequest WithFormParameters([NotNull]IEnumerable<KeyValuePair<string, string>> parameters)
        {
            _requestDataBytes = Encoding.UTF8.GetBytes(parameters.ToQueryString());
            _request.ContentType = "application/x-www-form-urlencoded";
            return this;
        }

        public HttpRequest WithJsonParameter<TEntity>([NotNull] TEntity entity)
        {
            _requestDataBytes = Encoding.UTF8.GetBytes(entity.ToJson());
            _request.ContentType = "application/json;charset=UTF-8";
            return this;
        }

        public HttpRequest WithParameters([NotNull] byte[] requestBytes)
            => WithParameters(requestBytes, null);

        public HttpRequest WithParameters([NotNull] byte[] requestBytes, string contentType)
        {
            _requestDataBytes = requestBytes;
            if (string.IsNullOrWhiteSpace(contentType))
            {
                contentType = "application/x-www-form-urlencoded";
            }
            _request.ContentType = contentType;
            return this;
        }

        #endregion Parameter

        #region AddFile

        public HttpRequest WithFile(string filePath, string fileKey = "file",
            IEnumerable<KeyValuePair<string, string>> formFields = null)
            => WithFile(Path.GetFileName(filePath), File.ReadAllBytes(filePath), fileKey, formFields);

        public HttpRequest WithFile(string fileName, byte[] fileBytes, string fileKey = "file",
            IEnumerable<KeyValuePair<string, string>> formFields = null)
        {
            var boundary = $"----------------------------{DateTime.Now.Ticks:X}";

            var boundaryBytes = Encoding.ASCII.GetBytes($"\r\n--{boundary}\r\n");
            var endBoundaryBytes = Encoding.ASCII.GetBytes($"\r\n--{boundary}--");
            using (var memStream = new MemoryStream())
            {
                if (formFields != null)
                {
                    foreach (var pair in formFields)
                    {
                        memStream.Write(Encoding.UTF8.GetBytes(string.Format(HttpHelper.FormDataFormat, pair.Key, pair.Value, boundary)));
                    }
                }
                memStream.Write(boundaryBytes);
                memStream.Write(Encoding.UTF8.GetBytes(string.Format(HttpHelper.FileHeaderFormat, fileKey, fileName)));
                memStream.Write(fileBytes);
                memStream.Write(endBoundaryBytes);
                _requestDataBytes = memStream.ToArray();
            }

            _request.ContentType = $"multipart/form-data; boundary={boundary}";
            _request.KeepAlive = true;

            return this;
        }

        public HttpRequest WithFiles(IEnumerable<string> filePaths, IEnumerable<KeyValuePair<string, string>> formFields = null)
            => WithFiles(
                filePaths.Select(_ => new KeyValuePair<string, byte[]>(
                    Path.GetFileName(_),
                    File.ReadAllBytes(_))),
                formFields);

        public HttpRequest WithFiles(IEnumerable<KeyValuePair<string, byte[]>> files,
            IEnumerable<KeyValuePair<string, string>> formFields = null)
        {
            var boundary = $"----------------------------{DateTime.Now.Ticks:X}";

            var boundaryBytes = Encoding.ASCII.GetBytes($"\r\n--{boundary}\r\n");
            var endBoundaryBytes = Encoding.ASCII.GetBytes($"\r\n--{boundary}--");

            using (var memStream = new MemoryStream())
            {
                if (formFields != null)
                {
                    foreach (var pair in formFields)
                    {
                        memStream.Write(Encoding.UTF8.GetBytes(string.Format(HttpHelper.FormDataFormat, pair.Key, pair.Value, boundary)));
                    }
                }

                foreach (var file in files)
                {
                    memStream.Write(boundaryBytes);

                    memStream.Write(Encoding.UTF8.GetBytes(string.Format(HttpHelper.FileHeaderFormat, Path.GetFileNameWithoutExtension(file.Key), file.Key)));
                    memStream.Write(file.Value);
                }

                memStream.Write(endBoundaryBytes);
                _requestDataBytes = memStream.ToArray();
            }

            _request.ContentType = $"multipart/form-data; boundary={boundary}";
            _request.KeepAlive = true;

            return this;
        }

        #endregion AddFile

        #region Other

        public HttpRequest AjaxRequest(bool isAjaxRequest)
        {
            _request.Headers["X-Requested-With"] = "XMLHttpRequest";
            return this;
        }

        #endregion Other

        #region Execute

        private void BuildRequest()
        {
            if (IsNeedRequestStream(_request.Method)
                && _requestDataBytes != null
                && _requestDataBytes.Length > 0)
            {
                _request.ContentLength = _requestDataBytes.Length;
                var requestStream = _request.GetRequestStream();
                requestStream.Write(_requestDataBytes);
            }
        }

        private async Task BuildRequestAsync()
        {
            if (IsNeedRequestStream(_request.Method)
                && _requestDataBytes != null
                && _requestDataBytes.Length > 0)
            {
                _request.ContentLength = _requestDataBytes.Length;
                var requestStream = await _request.GetRequestStreamAsync();
                await requestStream.WriteAsync(_requestDataBytes);
            }
        }

        public HttpWebResponse ExecuteForResponse()
        {
            BuildRequest();
            return (HttpWebResponse)_request.GetResponse();
        }

        public async Task<HttpWebResponse> ExecuteForResponseAsync()
        {
            await BuildRequestAsync();
            return (HttpWebResponse)(await _request.GetResponseAsync());
        }

        public byte[] ExecuteBytes()
        {
            BuildRequest();
            return _request.GetReponseBytesSafe();
        }

        public string Execute()
        {
            BuildRequest();
            return _request.GetReponseStringSafe();
        }

        public T Execute<T>() => Execute().StringToType<T>();

        public T Execute<T>(T defaultValue) => Execute().StringToType(defaultValue);

        public async Task<byte[]> ExecuteBytesAsync()
        {
            await BuildRequestAsync();
            return await _request.GetReponseBytesSafeAsync();
        }

        public async Task<string> ExecuteAsync()
        {
            await BuildRequestAsync();
            return await _request.GetReponseStringSafeAsync();
        }

        public Task<T> ExecuteAsync<T>() => ExecuteAsync().ContinueWith(_ => _.Result.StringToType<T>());

        public Task<T> ExecuteAsync<T>(T defaultValue) => ExecuteAsync().ContinueWith(_ => _.Result.StringToType<T>(defaultValue));

        #endregion Execute
    }
}
