using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WeihanLi.Common.Extensions;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers
{
    public class HttpResponse
    {
        public HttpStatusCode StatusCode { get; set; }

        public byte[] ResponseBytes { get; set; }

        public IDictionary<string, string> Headers { get; set; }
    }

    public class WebRequestHttpRequester : IHttpRequester
    {
        #region private fields

        private HttpWebRequest _request;
        private byte[] _requestDataBytes;
        private string _requestUrl;

        #endregion private fields

        #region ctor

        /// <summary>
        /// Create HttpRequest with GET Request Method
        /// </summary>
        /// <param name="requestUrl">requestUrl</param>
        public WebRequestHttpRequester(string requestUrl) : this(requestUrl, HttpMethod.Get)
        {
        }

        /// <summary>
        /// Create HttpRequest with specific Request Method
        /// </summary>
        /// <param name="requestUrl">requestUrl</param>
        /// <param name="queryDictionary">queryDictionary</param>
        /// <param name="method">method</param>
        public WebRequestHttpRequester(string requestUrl, IDictionary<string, string> queryDictionary, HttpMethod method)
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
        public WebRequestHttpRequester(string requestUrl, HttpMethod method)
        {
            _requestUrl = requestUrl;
            _request = WebRequest.CreateHttp(requestUrl);
            _request.UserAgent = HttpHelper.GetUserAgent();

            _request.Method = method.Method;
        }

        #endregion ctor

        public IHttpRequester WithUrl(string url)
        {
            _requestUrl = url;
            _request = WebRequest.CreateHttp(url);
            return this;
        }

        public IHttpRequester WithMethod(HttpMethod method)
        {
            _request.Method = method.Method;
            return this;
        }

        #region AddHeader

        public IHttpRequester WithHeaders([NotNull]NameValueCollection customHeaders) => WithHeaders(customHeaders.ToDictionary());

        public IHttpRequester WithHeaders([NotNull]IEnumerable<KeyValuePair<string, string>> customHeaders)
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
                    this.WithCookie(cookieCollection);
                    continue;
                }
                _request.Headers[header.Key] = header.Value;
            }
            return this;
        }

        #endregion AddHeader

        #region private method

        private static bool IsNeedRequestStream(string requestMethod) => requestMethod.EqualsIgnoreCase("POST") || requestMethod.EqualsIgnoreCase("PUT") || requestMethod.EqualsIgnoreCase("PATCH");

        #endregion private method

        #region UserAgent

        public IHttpRequester WithUserAgent(bool isMobile)
        {
            _request.UserAgent = HttpHelper.GetUserAgent(isMobile);
            return this;
        }

        public IHttpRequester WithUserAgent(string userAgent)
        {
            _request.UserAgent = userAgent;
            return this;
        }

        #endregion UserAgent

        #region Referer

        public IHttpRequester WithReferer(string referer)
        {
            _request.Referer = referer;
            return this;
        }

        #endregion Referer

        #region Proxy

        public IHttpRequester WithProxy([NotNull]IWebProxy proxy)
        {
            _request.Proxy = proxy;
            return this;
        }

        #endregion Proxy

        #region Cookie

        public IHttpRequester WithCookie(string url, Cookie cookie)
        {
            if (null == _request.CookieContainer)
            {
                _request.CookieContainer = new CookieContainer();
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                _request.CookieContainer.Add(cookie);
            }
            else
            {
                _request.CookieContainer.Add(new Uri(url), cookie);
            }
            return this;
        }

        public IHttpRequester WithCookie(string url, CookieCollection cookies)
        {
            if (null == _request.CookieContainer)
            {
                _request.CookieContainer = new CookieContainer();
            }

            if (string.IsNullOrWhiteSpace(url))
            {
                _request.CookieContainer.Add(cookies);
            }
            else
            {
                _request.CookieContainer.Add(new Uri(url), cookies);
            }
            return this;
        }

        #endregion Cookie

        #region Parameter

        public IHttpRequester WithFormParameters([NotNull] NameValueCollection parameters) => WithFormParameters(parameters.ToDictionary());

        public IHttpRequester WithFormParameters([NotNull]IEnumerable<KeyValuePair<string, string>> parameters)
        {
            _requestDataBytes = Encoding.UTF8.GetBytes(parameters.ToQueryString());
            _request.ContentType = "application/x-www-form-urlencoded";
            return this;
        }

        public IHttpRequester WithJsonParameter<TEntity>([NotNull] TEntity entity)
        {
            _requestDataBytes = Encoding.UTF8.GetBytes(entity.ToJson());
            _request.ContentType = "application/json;charset=UTF-8";
            return this;
        }

        public IHttpRequester WithXmlParameter<TEntity>([NotNull] TEntity entity)
        {
            _requestDataBytes = XmlDataSerializer.Instance.Value.Serialize(entity);
            _request.ContentType = "application/xml;charset=UTF-8";
            return this;
        }

        public IHttpRequester WithParameters([NotNull] byte[] requestBytes)
            => WithParameters(requestBytes, null);

        public IHttpRequester WithParameters([NotNull] byte[] requestBytes, string contentType)
        {
            _requestDataBytes = requestBytes;
            if (string.IsNullOrWhiteSpace(contentType))
            {
                contentType = "application/json;charset=utf-8";
            }
            _request.ContentType = contentType;
            return this;
        }

        #endregion Parameter

        #region AddFile

        public IHttpRequester WithFile(string filePath, string fileKey = "file",
            IEnumerable<KeyValuePair<string, string>> formFields = null)
            => WithFile(Path.GetFileName(filePath), File.ReadAllBytes(filePath), fileKey, formFields);

        public IHttpRequester WithFile(string fileName, byte[] fileBytes, string fileKey = "file",
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

        public IHttpRequester WithFiles(IEnumerable<string> filePaths, IEnumerable<KeyValuePair<string, string>> formFields = null)
            => WithFiles(
                filePaths.Select(_ => new KeyValuePair<string, byte[]>(
                    Path.GetFileName(_),
                    File.ReadAllBytes(_))),
                formFields);

        public IHttpRequester WithFiles(IEnumerable<KeyValuePair<string, byte[]>> files,
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

        public HttpResponse ExecuteForResponse()
        {
            BuildRequest();
            var response = (HttpWebResponse)_request.GetResponse();
            return new HttpResponse
            {
                Headers = response.Headers.ToDictionary(),
                StatusCode = response.StatusCode,
                ResponseBytes = response.ReadAllBytes()
            };
        }

        public async Task<HttpResponse> ExecuteForResponseAsync()
        {
            await BuildRequestAsync();
            var response = (HttpWebResponse)(await _request.GetResponseAsync());
            var responseBytes = await response.ReadAllBytesAsync();
            return new HttpResponse
            {
                Headers = response.Headers.ToDictionary(),
                StatusCode = response.StatusCode,
                ResponseBytes = responseBytes
            };
        }

        public byte[] ExecuteForBytes()
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

        public TEntity ExecuteForJson<TEntity>() => Execute().JsonToType<TEntity>();

        public TEntity ExecuteForXml<TEntity>() => XmlDataSerializer.Instance.Value.Deserialize<TEntity>(ExecuteForBytes());

        public T Execute<T>(T defaultValue) => Execute().StringToType(defaultValue);

        public async Task<byte[]> ExecuteForBytesAsync()
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

        public Task<TEntity> ExecuteForJsonAsync<TEntity>() => ExecuteAsync().ContinueWith(_ => _.Result.JsonToType<TEntity>());

        public Task<TEntity> ExecuteForXmlAsync<TEntity>() => ExecuteForBytesAsync().ContinueWith(_ => XmlDataSerializer.Instance.Value.Deserialize<TEntity>(_.Result));

        #endregion Execute
    }

    public class HttpClientHttpRequester : IHttpRequester
    {
        private static readonly string[] s_wellKnownContentHeaders = {
            HttpKnownHeaderNames.ContentDisposition,
            HttpKnownHeaderNames.ContentEncoding,
            HttpKnownHeaderNames.ContentLanguage,
            HttpKnownHeaderNames.ContentLength,
            HttpKnownHeaderNames.ContentLocation,
            HttpKnownHeaderNames.ContentMD5,
            HttpKnownHeaderNames.ContentRange,
            HttpKnownHeaderNames.ContentType,
            HttpKnownHeaderNames.Expires,
            HttpKnownHeaderNames.LastModified
        };

        private static bool IsWellKnownContentHeader(string header)
        {
            foreach (string contentHeaderName in s_wellKnownContentHeaders)
            {
                if (string.Equals(header, contentHeaderName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        private HttpClient Client;

        private readonly HttpRequestMessage _request = new HttpRequestMessage();

        private CookieContainer _cookieContainer = null;
        private IWebProxy _proxy = null;

        private void BuildHttpClient()
        {
            var handler = new HttpClientHandler() { };
            if (_proxy == null)
            {
                handler.UseProxy = false;
            }
            else
            {
                handler.UseProxy = true;
                handler.Proxy = _proxy;
            }
            if (_cookieContainer == null)
            {
                handler.UseCookies = false;
            }
            else
            {
                handler.UseCookies = true;
                handler.CookieContainer = _cookieContainer;
            }
            Client = new HttpClient(handler);
        }

        public string Execute()
        {
            BuildHttpClient();
            return Client.SendAsync(_request)
                .ContinueWith(res => res.Result.Content.ReadAsStringAsync().ContinueWith(r => r.Result))
                .Result.GetAwaiter().GetResult();
        }

        public async Task<string> ExecuteAsync()
        {
            BuildHttpClient();
            var response = await Client.SendAsync(_request);
            var result = await response.Content.ReadAsStringAsync();
            return result;
        }

        public HttpResponse ExecuteForResponse()
        {
            BuildHttpClient();
            var response = Client.SendAsync(_request).Result;
            var responseBytes = response.Content.ReadAsByteArrayAsync().Result;
            return new HttpResponse
            {
                Headers = response.Headers.ToDictionary(x => x.Key, x => x.Value.StringJoin(",")),
                ResponseBytes = responseBytes,
                StatusCode = response.StatusCode
            };
        }

        public async Task<HttpResponse> ExecuteForResponseAsync()
        {
            BuildHttpClient();
            var response = await Client.SendAsync(_request);
            var responseBytes = await response.Content.ReadAsByteArrayAsync();
            return new HttpResponse
            {
                Headers = response.Headers.ToDictionary(x => x.Key, x => x.Value.StringJoin(",")),
                ResponseBytes = responseBytes,
                StatusCode = response.StatusCode
            };
        }

        public IHttpRequester WithCookie(string url, Cookie cookie)
        {
            if (null == _cookieContainer)
            {
                _cookieContainer = new CookieContainer();
            }

            if (string.IsNullOrEmpty(url))
            {
                _cookieContainer.Add(cookie);
            }
            else
            {
                _cookieContainer.Add(new Uri(url), cookie);
            }
            return this;
        }

        public IHttpRequester WithCookie(string url, CookieCollection cookies)
        {
            if (null == _cookieContainer)
            {
                _cookieContainer = new CookieContainer();
            }
            if (string.IsNullOrWhiteSpace(url))
            {
                _cookieContainer.Add(cookies);
            }
            else
            {
                _cookieContainer.Add(new Uri(url), cookies);
            }
            return this;
        }

        public IHttpRequester WithProxy(IWebProxy proxy)
        {
            _proxy = proxy;
            return this;
        }

        public IHttpRequester WithFile(string fileName, byte[] fileBytes, string fileKey = "file", IEnumerable<KeyValuePair<string, string>> formFields = null)
        {
            var content = new MultipartFormDataContent($"form--{DateTime.Now.Ticks:X}");
            if (formFields != null)
            {
                foreach (var kv in formFields)
                {
                    content.Add(new StringContent(kv.Value), kv.Key);
                }
            }
            content.Add(new ByteArrayContent(fileBytes), fileKey, fileName);

            _request.Content = content;
            return this;
        }

        public IHttpRequester WithFiles(IEnumerable<KeyValuePair<string, byte[]>> files, IEnumerable<KeyValuePair<string, string>> formFields = null)
        {
            var content = new MultipartFormDataContent($"form--{DateTime.Now.Ticks:X}");
            if (formFields != null)
            {
                foreach (var kv in formFields)
                {
                    content.Add(new StringContent(kv.Value), kv.Key);
                }
            }

            files.ForEach((file, idx) => { content.Add(new ByteArrayContent(file.Value), $"{file.Key}_{idx}", file.Key); });

            _request.Content = content;
            return this;
        }

        public IHttpRequester WithUrl(string url)
        {
            _request.RequestUri = new Uri(url);
            return this;
        }

        public IHttpRequester WithMethod(HttpMethod method)
        {
            _request.Method = method;
            return this;
        }

        public IHttpRequester WithHeaders([CanBeNull] IEnumerable<KeyValuePair<string, string>> customHeaders)
        {
            if (customHeaders != null)
            {
                foreach (var header in customHeaders)
                {
                    // The System.Net.Http APIs require HttpRequestMessage headers to be properly divided between the request headers
                    // collection and the request content headers collection for all well-known header names.  And custom headers
                    // are only allowed in the request headers collection and not in the request content headers collection.
                    if (IsWellKnownContentHeader(header.Key))
                    {
                        if (_request.Content == null)
                        {
                            // Create empty content so that we can send the entity-body header.
                            _request.Content = new ByteArrayContent(new byte[0]);
                        }

                        _request.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                    else
                    {
                        _request.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }
                }
            }

            return this;
        }

        public IHttpRequester WithParameters([NotNull] byte[] requestBytes, string contentType)
        {
            _request.Content = new ByteArrayContent(requestBytes);
            _request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(contentType);
            return this;
        }

        public IHttpRequester WithReferer(string referer)
        {
            _request.Headers.Referrer = new Uri(referer);
            _request.Headers.TryAddWithoutValidation(HttpKnownHeaderNames.Referer, referer);
            return this;
        }

        public IHttpRequester WithUserAgent(string userAgent)
        {
            _request.Headers.TryAddWithoutValidation(HttpKnownHeaderNames.UserAgent, userAgent);
            return this;
        }
    }

    internal static partial class HttpKnownHeaderNames
    {
        // When adding a new constant, add it to HttpKnownHeaderNames.TryGetHeaderName.cs as well.

        public const string Accept = "Accept";
        public const string AcceptCharset = "Accept-Charset";
        public const string AcceptEncoding = "Accept-Encoding";
        public const string AcceptLanguage = "Accept-Language";
        public const string AcceptPatch = "Accept-Patch";
        public const string AcceptRanges = "Accept-Ranges";
        public const string AccessControlAllowCredentials = "Access-Control-Allow-Credentials";
        public const string AccessControlAllowHeaders = "Access-Control-Allow-Headers";
        public const string AccessControlAllowMethods = "Access-Control-Allow-Methods";
        public const string AccessControlAllowOrigin = "Access-Control-Allow-Origin";
        public const string AccessControlExposeHeaders = "Access-Control-Expose-Headers";
        public const string AccessControlMaxAge = "Access-Control-Max-Age";
        public const string Age = "Age";
        public const string Allow = "Allow";
        public const string AltSvc = "Alt-Svc";
        public const string Authorization = "Authorization";
        public const string CacheControl = "Cache-Control";
        public const string Connection = "Connection";
        public const string ContentDisposition = "Content-Disposition";
        public const string ContentEncoding = "Content-Encoding";
        public const string ContentLanguage = "Content-Language";
        public const string ContentLength = "Content-Length";
        public const string ContentLocation = "Content-Location";
        public const string ContentMD5 = "Content-MD5";
        public const string ContentRange = "Content-Range";
        public const string ContentSecurityPolicy = "Content-Security-Policy";
        public const string ContentType = "Content-Type";
        public const string Cookie = "Cookie";
        public const string Cookie2 = "Cookie2";
        public const string Date = "Date";
        public const string ETag = "ETag";
        public const string Expect = "Expect";
        public const string Expires = "Expires";
        public const string From = "From";
        public const string Host = "Host";
        public const string IfMatch = "If-Match";
        public const string IfModifiedSince = "If-Modified-Since";
        public const string IfNoneMatch = "If-None-Match";
        public const string IfRange = "If-Range";
        public const string IfUnmodifiedSince = "If-Unmodified-Since";
        public const string KeepAlive = "Keep-Alive";
        public const string LastModified = "Last-Modified";
        public const string Link = "Link";
        public const string Location = "Location";
        public const string MaxForwards = "Max-Forwards";
        public const string Origin = "Origin";
        public const string P3P = "P3P";
        public const string Pragma = "Pragma";
        public const string ProxyAuthenticate = "Proxy-Authenticate";
        public const string ProxyAuthorization = "Proxy-Authorization";
        public const string ProxyConnection = "Proxy-Connection";
        public const string PublicKeyPins = "Public-Key-Pins";
        public const string Range = "Range";
        public const string Referer = "Referer"; // NB: The spelling-mistake "Referer" for "Referrer" must be matched.
        public const string RetryAfter = "Retry-After";
        public const string SecWebSocketAccept = "Sec-WebSocket-Accept";
        public const string SecWebSocketExtensions = "Sec-WebSocket-Extensions";
        public const string SecWebSocketKey = "Sec-WebSocket-Key";
        public const string SecWebSocketProtocol = "Sec-WebSocket-Protocol";
        public const string SecWebSocketVersion = "Sec-WebSocket-Version";
        public const string Server = "Server";
        public const string SetCookie = "Set-Cookie";
        public const string SetCookie2 = "Set-Cookie2";
        public const string StrictTransportSecurity = "Strict-Transport-Security";
        public const string TE = "TE";
        public const string TSV = "TSV";
        public const string Trailer = "Trailer";
        public const string TransferEncoding = "Transfer-Encoding";
        public const string Upgrade = "Upgrade";
        public const string UpgradeInsecureRequests = "Upgrade-Insecure-Requests";
        public const string UserAgent = "User-Agent";
        public const string Vary = "Vary";
        public const string Via = "Via";
        public const string WWWAuthenticate = "WWW-Authenticate";
        public const string Warning = "Warning";
        public const string XAspNetVersion = "X-AspNet-Version";
        public const string XContentDuration = "X-Content-Duration";
        public const string XContentTypeOptions = "X-Content-Type-Options";
        public const string XFrameOptions = "X-Frame-Options";
        public const string XMSEdgeRef = "X-MSEdge-Ref";
        public const string XPoweredBy = "X-Powered-By";
        public const string XRequestID = "X-Request-ID";
        public const string XUACompatible = "X-UA-Compatible";
    }
}
