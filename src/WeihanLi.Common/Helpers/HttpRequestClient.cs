using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers
{
    public class HttpRequestClient
    {
        #region private fields

        private readonly HttpWebRequest _request;
        private byte[] _requestDataBytes;
        private readonly string _requestUrl;

        #endregion private fields

        #region ctor

        /// <summary>
        /// Create HttpRequestClient with GET Request Method
        /// </summary>
        /// <param name="requestUrl">requestUrl</param>
        public HttpRequestClient(string requestUrl) : this(requestUrl, "GET")
        {
        }

        public HttpRequestClient(string requestUrl, string method)
        {
            _requestUrl = requestUrl;
            _request = WebRequest.CreateHttp(requestUrl);
            _request.UserAgent = HttpHelper.GetUserAgent();
            if (!string.IsNullOrWhiteSpace(method))
            {
                _request.Method = method.ToUpper();
            }
        }

        #endregion ctor

        #region AddHeader

        public void AddCustomHeaders([NotNull]NameValueCollection customHeaders) => AddCustomHeaders(customHeaders.ToDictionary());

        public void AddCustomHeaders([NotNull]IEnumerable<KeyValuePair<string, string>> customHeaders)
        {
            foreach (var header in customHeaders)
            {
                if (header.Key.EqualsIgnoreCase("REFERER"))
                {
                    SetReferer(header.Value);
                    continue;
                }
                if (header.Key.EqualsIgnoreCase("User-Agent"))
                {
                    SetUserAgent(header.Value);
                    continue;
                }
                if (header.Key.EqualsIgnoreCase("COOKIE"))
                {
                    var cookieCollection = new CookieCollection();
                    var host = new Uri(_requestUrl).Host;
                    header.Value.Split(';').ForEach(c => cookieCollection.Add(new Cookie(c.Split('=')[0].Trim(), c.Split('=')[1].Trim(), "/", host)));
                    AddCookies(cookieCollection);
                    continue;
                }
                _request.Headers[header.Key] = header.Value;
            }
        }

        #endregion AddHeader

        #region private method

        private static bool IsNeedRequestStream(string requestMethod) => requestMethod.EqualsIgnoreCase("POST");

        #endregion private method

        #region UserAgent

        public void SetUserAgent(bool isMobile)
        {
            _request.UserAgent = HttpHelper.GetUserAgent(isMobile);
        }

        public void SetUserAgent(string userAgent)
        {
            _request.UserAgent = userAgent;
        }

        #endregion UserAgent

        #region Referer

        public void SetReferer(string referer)
        {
            _request.Referer = referer;
        }

        #endregion Referer

        #region Proxy

        public void AddProxy(string url)
        {
            _request.Proxy = new WebProxy(new Uri(url));
        }

        public void AddProxy(string url, string userName, string password)
        {
            _request.Proxy = new WebProxy(new Uri(url))
            {
                Credentials = new NetworkCredential(userName, password)
            };
        }

        public void AddProxy(WebProxy proxy)
        {
            _request.Proxy = proxy;
        }

        #endregion Proxy

        #region Cookie

        public void AddCookie(Cookie cookie)
        {
            if (null == _request.CookieContainer)
            {
                _request.CookieContainer = new CookieContainer();
            }
            _request.CookieContainer.Add(cookie);
        }

        public void AddCookie(string url, Cookie cookie)
        {
            if (null == _request.CookieContainer)
            {
                _request.CookieContainer = new CookieContainer();
            }
            _request.CookieContainer.Add(new Uri(url), cookie);
        }

        public void AddCookies(CookieCollection cookies)
        {
            if (null == _request.CookieContainer)
            {
                _request.CookieContainer = new CookieContainer();
            }
            _request.CookieContainer.Add(cookies);
        }

        public void AddCookies(string url, CookieCollection cookies)
        {
            if (null == _request.CookieContainer)
            {
                _request.CookieContainer = new CookieContainer();
            }
            _request.CookieContainer.Add(new Uri(url), cookies);
        }

        #endregion Cookie

        #region Parameter

        public void AddParameters([NotNull] NameValueCollection parameters) => AddParameters(parameters.ToDictionary());

        public void AddParameters([NotNull]IEnumerable<KeyValuePair<string, string>> parameters)
        {
            _requestDataBytes = Encoding.UTF8.GetBytes(parameters.ToQueryString());
            _request.ContentType = "application/x-www-form-urlencoded";
        }

        public void AddEntityParameter<TEntity>([NotNull] TEntity entity)
        {
            _requestDataBytes = Encoding.UTF8.GetBytes(entity.ToJson());
            _request.ContentType = "application/json;charset=UTF-8";
        }

        public void AddParameters([NotNull] byte[] requestBytes)
            => AddParameters(requestBytes, null);

        public void AddParameters([NotNull] byte[] requestBytes, string contentType)
        {
            _requestDataBytes = requestBytes;
            if (string.IsNullOrWhiteSpace(contentType))
            {
                contentType = "application/x-www-form-urlencoded";
            }
            _request.ContentType = contentType;
        }

        #endregion Parameter

        #region AddFile

        public void AddFile(string filePath, string fileKey = "file",
            IEnumerable<KeyValuePair<string, string>> formFields = null)
            => AddFile(Path.GetFileName(filePath), File.ReadAllBytes(filePath), fileKey, formFields);

        public void AddFile(string fileName, byte[] fileBytes, string fileKey = "file",
            IEnumerable<KeyValuePair<string, string>> formFields = null)
        {
            var boundary = $"----------------------------{DateTime.Now.Ticks:X}";

            var boundarybytes = Encoding.ASCII.GetBytes($"\r\n--{boundary}\r\n");
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

                memStream.Write(boundarybytes);

                memStream.Write(Encoding.UTF8.GetBytes(string.Format(HttpHelper.FileHeaderFormat, fileKey, fileName)));

                memStream.Write(fileBytes);

                memStream.Write(endBoundaryBytes);

                _requestDataBytes = memStream.ToArray();
            }

            _request.ContentType = $"multipart/form-data; boundary={boundary}";
            _request.Method = "POST";
            _request.KeepAlive = true;
        }

        public void AddFiles(IEnumerable<string> filePaths, IEnumerable<KeyValuePair<string, string>> formFields = null)
            => AddFiles(
                filePaths.Select(_ => new KeyValuePair<string, byte[]>(Path.GetFileName(_), File.ReadAllBytes(_))),
                formFields);

        public void AddFiles(IEnumerable<KeyValuePair<string, byte[]>> files,
            IEnumerable<KeyValuePair<string, string>> formFields = null)
        {
            var boundary = $"----------------------------{DateTime.Now.Ticks:X}";

            var boundarybytes = Encoding.ASCII.GetBytes($"\r\n--{boundary}\r\n");
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
                    memStream.Write(boundarybytes);

                    memStream.Write(Encoding.UTF8.GetBytes(string.Format(HttpHelper.FileHeaderFormat, Path.GetFileNameWithoutExtension(file.Key), file.Key)));
                    memStream.Write(file.Value);
                }

                memStream.Write(endBoundaryBytes);
                _requestDataBytes = memStream.ToArray();
            }

            _request.ContentType = $"multipart/form-data; boundary={boundary}";
            _request.Method = "POST";
            _request.KeepAlive = true;
        }

        #endregion AddFile

        #region Execute

        public HttpWebResponse ExecuteForResponse()
        {
            if (IsNeedRequestStream(_request.Method))
            {
                _request.ContentLength = _requestDataBytes.Length;
                var requestStream = _request.GetRequestStream();
                requestStream.Write(_requestDataBytes);
            }
            return (HttpWebResponse)_request.GetResponse();
        }

        public async Task<HttpWebResponse> ExecuteForResponseAsync()
        {
            if (IsNeedRequestStream(_request.Method))
            {
                _request.ContentLength = _requestDataBytes.Length;
                var requestStream = await _request.GetRequestStreamAsync();
                await requestStream.WriteAsync(_requestDataBytes);
            }
            return (HttpWebResponse)(await _request.GetResponseAsync());
        }

        public byte[] ExecuteBytes()
        {
            if (IsNeedRequestStream(_request.Method))
            {
                _request.ContentLength = _requestDataBytes.Length;
                var requestStream = _request.GetRequestStream();
                requestStream.Write(_requestDataBytes);
            }
            return _request.GetReponseBytesSafe();
        }

        public string Execute()
        {
            if (IsNeedRequestStream(_request.Method))
            {
                _request.ContentLength = _requestDataBytes.Length;
                var requestStream = _request.GetRequestStream();
                requestStream.Write(_requestDataBytes);
            }
            return _request.GetReponseStringSafe();
        }

        public T Execute<T>() => Execute().StringToType<T>();

        public async Task<byte[]> ExecuteBytesAsync()
        {
            if (IsNeedRequestStream(_request.Method))
            {
                _request.ContentLength = _requestDataBytes.Length;
                var requestStream = await _request.GetRequestStreamAsync();
                await requestStream.WriteAsync(_requestDataBytes);
            }
            return await _request.GetReponseBytesSafeAsync();
        }

        public async Task<string> ExecuteAsync()
        {
            if (IsNeedRequestStream(_request.Method))
            {
                _request.ContentLength = _requestDataBytes.Length;
                var requestStream = await _request.GetRequestStreamAsync();
                await requestStream.WriteAsync(_requestDataBytes);
            }
            return await _request.GetReponseStringSafeAsync();
        }

        public Task<T> ExecuteAsync<T>() => ExecuteAsync().ContinueWith(_ => _.Result.StringToType<T>());

        #endregion Execute
    }
}
