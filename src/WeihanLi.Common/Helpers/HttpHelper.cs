using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WeihanLi.Common.Http;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers
{
    /// <summary>
    /// HTTP请求帮助类
    /// </summary>
    public static class HttpHelper
    {
        #region Constants

        #region UploadFileHeaderFormat

        /// <summary>
        /// 文件头 format
        /// 0:fileKey
        /// 1:fileName
        /// </summary>
        internal const string FileHeaderFormat =
            "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
            "Content-Type: application/octet-stream\r\n\r\n";

        /// <summary>
        /// FormDataFormat
        /// 0:key
        /// 1:value
        /// 2:boundary
        /// </summary>
        internal const string FormDataFormat = "\r\n--{2}\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";

        #endregion UploadFileHeaderFormat

        #endregion Constants

        /// <summary>
        /// Content Header
        /// get latest from https://github.com/dotnet/corefx/blob/master/src/System.Net.Requests/src/System/Net/HttpWebRequest.cs#L1420
        /// </summary>
        public static readonly HashSet<string> WellKnownContentHeaders = new(StringComparer.OrdinalIgnoreCase)
        {
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

        public static bool IsWellKnownContentHeader(string header)
        {
            return WellKnownContentHeaders.Contains(header);
        }

        #region WebRequest

        #region HttpGet

        /// <summary>
        /// HTTP GET请求，返回字符串
        /// </summary>
        /// <param name="url"> url </param>
        /// <returns></returns>"
        public static string HttpGetString(string url)
            => HttpGetString(url, null, null);

        /// <summary>
        /// HTTP GET请求，返回字符串
        /// </summary>
        /// <param name="url"> url </param>
        /// <param name="customHeaders"></param>
        /// <returns></returns>"
        public static string HttpGetString(string url, IEnumerable<KeyValuePair<string, string>>? customHeaders)
            => HttpGetString(url, customHeaders, null);

        /// <summary>
        /// HTTP GET请求，返回字符串
        /// </summary>
        /// <param name="url"> url </param>
        /// <param name="proxy">proxy</param>
        /// <param name="customHeaders">customHeaders</param>
        /// <returns></returns>"
        public static string HttpGetString(string url, IEnumerable<KeyValuePair<string, string>>? customHeaders, WebProxy? proxy)
        {
            var request = WebRequest.CreateHttp(url);
            request.UserAgent = GetUserAgent();
            request.Method = "GET";

            if (null != customHeaders)
            {
                foreach (var header in customHeaders)
                {
                    if (header.Key.EqualsIgnoreCase("REFERER"))
                    {
                        request.Referer = header.Value;
                        continue;
                    }
                    if (header.Key.EqualsIgnoreCase("User-Agent"))
                    {
                        request.UserAgent = header.Value;
                        continue;
                    }
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (null != proxy)
            {
                request.Proxy = proxy;
            }
            return request.GetReponseString();
        }

        /// <summary>
        /// HTTP GET请求，返回字符串
        /// </summary>
        /// <param name="url"> url </param>
        /// <returns></returns>
        public static Task<string> HttpGetStringAsync(string url)
            => HttpGetStringAsync(url, null, null);

        /// <summary>
        /// HTTP GET请求，返回字符串
        /// </summary>
        /// <param name="url"> url </param>
        /// <param name="customHeaders"></param>
        /// <returns></returns>"
        public static Task<string> HttpGetStringAsync(string url, IEnumerable<KeyValuePair<string, string>>? customHeaders)
            => HttpGetStringAsync(url, customHeaders, null);

        /// <summary>
        /// HTTP GET请求，返回字符串
        /// </summary>
        /// <param name="url"> url </param>
        /// <param name="customHeaders"></param>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public static async Task<string> HttpGetStringAsync(string url, IEnumerable<KeyValuePair<string, string>>? customHeaders,
            WebProxy? proxy)
        {
            var request = WebRequest.CreateHttp(url);
            request.UserAgent = GetUserAgent();
            request.Method = "GET";
            if (null != customHeaders)
            {
                foreach (var header in customHeaders)
                {
                    if (header.Key.EqualsIgnoreCase("REFERER"))
                    {
                        request.Referer = header.Value;
                        continue;
                    }
                    if (header.Key.EqualsIgnoreCase("User-Agent"))
                    {
                        request.UserAgent = header.Value;
                        continue;
                    }
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            if (null != proxy)
            {
                request.Proxy = proxy;
            }
            return await request.GetReponseStringSafeAsync();
        }

        /// <summary>
        /// HTTP GET请求，返回字节数组
        /// </summary>
        /// <param name="url"> url </param>
        /// <returns></returns>"
        public static byte[] HttpGetForBytes(string url)
            => HttpGetForBytes(url, null, null);

        /// <summary>
        /// HTTP GET请求，返回字节数组
        /// </summary>
        /// <param name="url"> url </param>
        /// <param name="customHeaders">customHeaders</param>
        /// <returns></returns>"
        public static byte[] HttpGetForBytes(string url, IEnumerable<KeyValuePair<string, string>>? customHeaders)
            => HttpGetForBytes(url, customHeaders, null);

        /// <summary>
        /// HTTP GET请求，返回字节数组
        /// </summary>
        /// <param name="url"></param>
        /// <param name="customHeaders"></param>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public static byte[] HttpGetForBytes(string url, IEnumerable<KeyValuePair<string, string>>? customHeaders,
            WebProxy? proxy)
        {
            var request = WebRequest.CreateHttp(url);
            request.UserAgent = GetUserAgent();
            request.Method = "GET";

            if (null != customHeaders)
            {
                foreach (var header in customHeaders)
                {
                    if (header.Key.EqualsIgnoreCase("REFERER"))
                    {
                        request.Referer = header.Value;
                        continue;
                    }
                    if (header.Key.EqualsIgnoreCase("User-Agent"))
                    {
                        request.UserAgent = header.Value;
                        continue;
                    }
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (null != proxy)
            {
                request.Proxy = proxy;
            }

            return request.GetReponseBytesSafe();
        }

        public static Task<byte[]> HttpGetForBytesAsync(string url) => HttpGetForBytesAsync(url, null, null);

        public static Task<byte[]> HttpGetForBytesAsync(string url, IEnumerable<KeyValuePair<string, string>>? customHeaders) => HttpGetForBytesAsync(url, customHeaders, null);

        /// <summary>
        /// HTTP GET请求，返回字节数组
        /// </summary>
        /// <param name="url"></param>
        /// <param name="customHeaders"></param>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public static async Task<byte[]> HttpGetForBytesAsync(string url, IEnumerable<KeyValuePair<string, string>>? customHeaders,
            WebProxy? proxy)
        {
            var request = WebRequest.CreateHttp(url);
            request.UserAgent = GetUserAgent();
            request.Method = "GET";

            if (null != customHeaders)
            {
                foreach (var header in customHeaders)
                {
                    if (header.Key.EqualsIgnoreCase("REFERER"))
                    {
                        request.Referer = header.Value;
                        continue;
                    }
                    if (header.Key.EqualsIgnoreCase("User-Agent"))
                    {
                        request.UserAgent = header.Value;
                        continue;
                    }
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (null != proxy)
            {
                request.Proxy = proxy;
            }

            return await request.GetReponseBytesSafeAsync();
        }

        public static T HttpGetFor<T>(string url)
            => HttpGetString(url).StringToType<T>();

        public static T HttpGetFor<T>(string url, IEnumerable<KeyValuePair<string, string>>? customHeaders)
            => HttpGetString(url, customHeaders).StringToType<T>();

        public static T HttpGetFor<T>(string url, IEnumerable<KeyValuePair<string, string>>? customHeaders,
            WebProxy? proxy)
            => HttpGetString(url, customHeaders, proxy).StringToType<T>();

        public static Task<T> HttpGetForAsync<T>(string url)
            => HttpGetStringAsync(url).ContinueWith(result => result.Result.StringToType<T>());

        public static Task<T> HttpGetForAsync<T>(string url, IEnumerable<KeyValuePair<string, string>>? customHeaders)
            => HttpGetStringAsync(url, customHeaders).ContinueWith(result => result.Result.StringToType<T>());

        public static Task<T> HttpGetForAsync<T>(string url, IEnumerable<KeyValuePair<string, string>>? customHeaders,
            WebProxy? proxy)
            => HttpGetStringAsync(url, customHeaders, proxy).ContinueWith(result => result.Result.StringToType<T>());

        /// <summary>
        /// HTTP GET 请求，返回字符串
        /// </summary>
        /// <param name="url"> url </param>
        /// <param name="parameters"> post数据字典 </param>
        /// <returns></returns>
        public static string HttpGetString(string url, IDictionary<string, string>? parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                url = url + (url.IndexOf('?') < 0 ? "?" : "&") + string.Join("&", parameters.Select(p => $"{WebUtility.UrlEncode(p.Key)}={WebUtility.UrlEncode(p.Value)}"));
            }
            return HttpGetString(url);
        }

        /// <summary>
        /// HTTP GET 请求，返回字符串
        /// </summary>
        /// <param name="url"> url </param>
        /// <param name="parameters"> post数据字典 </param>
        /// <returns></returns>
        public static async Task<string> HttpGetStringAsync(string url, IDictionary<string, string>? parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                url = url + (url.IndexOf('?') < 0 ? "?" : "&") + string.Join("&", parameters.Select(p => $"{WebUtility.UrlEncode(p.Key)}={WebUtility.UrlEncode(p.Value)}"));
            }
            return await HttpGetStringAsync(url);
        }

        public static byte[] HttpGetForBytes(string url, IDictionary<string, string>? parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                url = url + (url.IndexOf('?') < 0 ? "?" : "&") + string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
            }
            return HttpGetForBytes(url);
        }

        public static async Task<byte[]> HttpGetForBytesAsync(string url, IDictionary<string, string>? parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                url = url + (url.IndexOf('?') < 0 ? "?" : "&") + string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
            }
            return await HttpGetForBytesAsync(url);
        }

        public static T HttpGetFor<T>(string url, IDictionary<string, string>? parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                url = url + (url.IndexOf('?') < 0 ? "?" : "&") + string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
            }
            return HttpGetFor<T>(url);
        }

        public static async Task<T> HttpGetForAsync<T>(string url, IDictionary<string, string>? parameters)
        {
            if (parameters != null && parameters.Count > 0)
            {
                url = url + (url.IndexOf('?') < 0 ? "?" : "&") + string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
            }
            return await HttpGetForAsync<T>(url);
        }

        #endregion HttpGet

        #region HttpPost

        /// <summary>
        /// 获取 post 请求的 ContentType
        /// </summary>
        /// <param name="isJsonFormat">请求参数是否是Json格式</param>
        /// <returns></returns>
        private static string GetContentType(bool isJsonFormat) => isJsonFormat ? "application/json;charset=UTF-8" : "application/x-www-form-urlencoded;charset=UTF-8";

        /// <summary>
        /// HTTP POST 请求，返回字符串
        /// </summary>
        /// <param name="url"> url </param>
        /// <param name="parameters"> post数据字典 </param>
        /// <returns></returns>
        public static string HttpPost(string url, IDictionary<string, string>? parameters)
            => HttpPost(url,
                Encoding.UTF8.GetBytes(string.Join("&",
                    parameters?.Select(p => $"{WebUtility.UrlEncode(p.Key)}={WebUtility.UrlEncode(p.Value)}") ?? Array.Empty<string>())), false);

        /// <summary>
        /// HTTP POST 请求，返回字符串
        /// </summary>
        /// <param name="url"> url </param>
        /// <param name="parameters"> post数据字典 </param>
        /// <returns></returns>
        public static Task<string> HttpPostAsync(string url, IDictionary<string, string>? parameters)
            => HttpPostAsync(url,
            Encoding.UTF8.GetBytes(string.Join("&",
        parameters?.Select(p => $"{WebUtility.UrlEncode(p.Key)}={WebUtility.UrlEncode(p.Value)}") ?? Array.Empty<string>())), false);

        /// <summary>
        /// Http
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string HttpPostJson<T>(string url, T data)
        => HttpPost(url, Encoding.UTF8.GetBytes(data.ToJson()));

        public static string HttpPostJson<T>(string url, T data, Encoding encoding)
        => HttpPost(url, encoding.GetBytes(data.ToJson()));

        /// <summary>
        /// HttpPostJsonAsync
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Task<string> HttpPostJsonAsync<T>(string url, T data)
            => HttpPostAsync(url, Encoding.UTF8.GetBytes(data.ToJson()));

        public static Task<string> HttpPostJsonAsync<T>(string url, T data, Encoding encoding)
            => HttpPostAsync(url, encoding.GetBytes(data.ToJson()));

        public static TResponse HttpPostJsonFor<TRequest, TResponse>(string url, TRequest data)
        => HttpPostFor<TResponse>(url, Encoding.UTF8.GetBytes(data.ToJson()), true);

        public static TResponse HttpPostJsonFor<TRequest, TResponse>(string url, TRequest data, Encoding encoding)
        => HttpPostFor<TResponse>(url, encoding.GetBytes(data.ToJson()), true);

        public static Task<TResponse> HttpPostJsonForAsync<TRequest, TResponse>(string url, TRequest data)
            => HttpPostForAsync<TResponse>(url, Encoding.UTF8.GetBytes(data.ToJson()), true);

        public static Task<TResponse> HttpPostJsonForAsync<TRequest, TResponse>(string url, TRequest data, Encoding encoding)
            => HttpPostForAsync<TResponse>(url, encoding.GetBytes(data.ToJson()), true);

        /// <summary>
        /// HTTP POST 请求，返回字符串
        /// </summary>
        /// <param name="url"> url </param>
        /// <param name="postData"> post数据 </param>
        /// <param name="isJsonFormat"> 是否是json格式数据 </param>
        /// <param name="customHeaders"></param>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public static string HttpPost(string url, byte[] postData, bool isJsonFormat = true, IEnumerable<KeyValuePair<string, string>>? customHeaders = null, WebProxy? proxy = null)
        {
            var request = WebRequest.CreateHttp(url);
            request.UserAgent = GetUserAgent();
            request.Method = "POST";

            request.ContentType = GetContentType(isJsonFormat);
            if (null != customHeaders)
            {
                foreach (var header in customHeaders)
                {
                    if (header.Key.EqualsIgnoreCase("REFERER"))
                    {
                        request.Referer = header.Value;
                        continue;
                    }
                    if (header.Key.EqualsIgnoreCase("User-Agent"))
                    {
                        request.UserAgent = header.Value;
                        continue;
                    }
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (null != proxy)
            {
                request.Proxy = proxy;
            }
            var postStream = request.GetRequestStream();
            postStream.Write(postData);
            return request.GetReponseStringSafe();
        }

        /// <summary>
        /// HTTP POST 请求，返回字符串
        /// </summary>
        /// <param name="url"> url </param>
        /// <param name="postData"> post数据 </param>
        /// <param name="isJsonFormat"> 是否是json格式数据 </param>
        /// <param name="customHeaders"></param>
        /// <param name="proxy"></param>
        /// <returns></returns>
        public static async Task<string> HttpPostAsync(string url, byte[] postData, bool isJsonFormat = true, IEnumerable<KeyValuePair<string, string>>? customHeaders = null, WebProxy? proxy = null)
        {
            var request = WebRequest.CreateHttp(url);
            request.UserAgent = GetUserAgent();
            request.Method = "POST";

            request.ContentType = GetContentType(isJsonFormat);
            if (null != customHeaders)
            {
                foreach (var header in customHeaders)
                {
                    if (header.Key.EqualsIgnoreCase("REFERER"))
                    {
                        request.Referer = header.Value;
                        continue;
                    }
                    if (header.Key.EqualsIgnoreCase("User-Agent"))
                    {
                        request.UserAgent = header.Value;
                        continue;
                    }
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (null != proxy)
            {
                request.Proxy = proxy;
            }
            var postStream = await request.GetRequestStreamAsync();
            await postStream.WriteAsync(postData);
            return await request.GetReponseStringSafeAsync();
        }

        public static async Task<string> HttpPostAsync(string url, byte[] postData, string contentType, IEnumerable<KeyValuePair<string, string>>? customHeaders = null, WebProxy? proxy = null)
        {
            var request = WebRequest.CreateHttp(url);
            request.UserAgent = GetUserAgent();
            request.Method = "POST";

            request.ContentType = contentType;

            if (null != customHeaders)
            {
                foreach (var header in customHeaders)
                {
                    if (header.Key.EqualsIgnoreCase("REFERER"))
                    {
                        request.Referer = header.Value;
                        continue;
                    }
                    if (header.Key.EqualsIgnoreCase("User-Agent"))
                    {
                        request.UserAgent = header.Value;
                        continue;
                    }
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (null != proxy)
            {
                request.Proxy = proxy;
            }

            var postStream = await request.GetRequestStreamAsync();
            await postStream.WriteAsync(postData);
            return await request.GetReponseStringSafeAsync();
        }

        public static T HttpPostFor<T>(string url, byte[] postData, bool isJsonFormat)
            => HttpPost(url, postData, isJsonFormat).StringToType<T>();

        public static async Task<T> HttpPostForAsync<T>(string url, byte[] postData, bool isJsonFormat)
            => (await HttpPostAsync(url, postData, isJsonFormat)).StringToType<T>();

        public static byte[] HttpPostForBytes(string url, byte[] postData, bool isJsonFormat, IEnumerable<KeyValuePair<string, string>>? customHeaders = null, WebProxy? proxy = null)
        {
            var request = WebRequest.CreateHttp(url);
            request.UserAgent = GetUserAgent();
            request.Method = "POST";

            request.ContentType = GetContentType(isJsonFormat);

            if (null != customHeaders)
            {
                foreach (var header in customHeaders)
                {
                    if (header.Key.EqualsIgnoreCase("REFERER"))
                    {
                        request.Referer = header.Value;
                        continue;
                    }
                    if (header.Key.EqualsIgnoreCase("User-Agent"))
                    {
                        request.UserAgent = header.Value;
                        continue;
                    }
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (null != proxy)
            {
                request.Proxy = proxy;
            }

            var postStream = request.GetRequestStream();
            postStream.Write(postData);

            return request.GetReponseBytesSafe();
        }

        public static byte[] HttpPostForBytes(string url, byte[] postData, string contentType, IEnumerable<KeyValuePair<string, string>>? customHeaders = null, WebProxy? proxy = null)
        {
            var request = WebRequest.CreateHttp(url);
            request.UserAgent = GetUserAgent();
            request.Method = "POST";

            request.ContentType = contentType;

            if (null != customHeaders)
            {
                foreach (var header in customHeaders)
                {
                    if (header.Key.EqualsIgnoreCase("REFERER"))
                    {
                        request.Referer = header.Value;
                        continue;
                    }
                    if (header.Key.EqualsIgnoreCase("User-Agent"))
                    {
                        request.UserAgent = header.Value;
                        continue;
                    }
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (null != proxy)
            {
                request.Proxy = proxy;
            }
            var postStream = request.GetRequestStream();
            postStream.Write(postData);
            return request.GetReponseBytesSafe();
        }

        public static async Task<byte[]> HttpPostForBytesAsync(string url, byte[] postData, bool isJsonFormat, IEnumerable<KeyValuePair<string, string>>? customHeaders = null, WebProxy? proxy = null)
        {
            var request = WebRequest.CreateHttp(url);
            request.UserAgent = GetUserAgent();
            request.Method = "POST";

            request.ContentType = GetContentType(isJsonFormat);
            if (null != customHeaders)
            {
                foreach (var header in customHeaders)
                {
                    if (header.Key.EqualsIgnoreCase("REFERER"))
                    {
                        request.Referer = header.Value;
                        continue;
                    }
                    if (header.Key.EqualsIgnoreCase("User-Agent"))
                    {
                        request.UserAgent = header.Value;
                        continue;
                    }
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (null != proxy)
            {
                request.Proxy = proxy;
            }
            var postStream = await request.GetRequestStreamAsync();
            await postStream.WriteAsync(postData);
            return await request.GetReponseBytesSafeAsync();
        }

        public static async Task<byte[]> HttpPostForBytesAsync(string url, byte[] postData, string contentType, IEnumerable<KeyValuePair<string, string>>? customHeaders = null, WebProxy? proxy = null)
        {
            var request = WebRequest.CreateHttp(url);
            request.UserAgent = GetUserAgent();
            request.Method = "POST";

            request.ContentType = contentType;
            if (null != customHeaders)
            {
                foreach (var header in customHeaders)
                {
                    if (header.Key.EqualsIgnoreCase("REFERER"))
                    {
                        request.Referer = header.Value;
                        continue;
                    }
                    if (header.Key.EqualsIgnoreCase("User-Agent"))
                    {
                        request.UserAgent = header.Value;
                        continue;
                    }
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            if (null != proxy)
            {
                request.Proxy = proxy;
            }
            var postStream = await request.GetRequestStreamAsync();
            await postStream.WriteAsync(postData);
            return await request.GetReponseBytesSafeAsync();
        }

        /// <summary>
        /// PostFile
        /// <see href="https://stackoverflow.com/questions/566462/upload-files-with-httpwebrequest-multipart-form-data"></see>
        /// <see href="http://www.cnblogs.com/greenerycn/archive/2010/05/15/csharp_http_post.html"></see>
        /// </summary>
        /// <param name="url">post url</param>
        /// <param name="filePath">filePath</param>
        /// <param name="fileKey">fileKey in form,default is "file"</param>
        /// <param name="formFields">other form fields</param>
        /// <param name="headers">headers</param>
        /// <returns></returns>
        public static string HttpPostFile(string url, string filePath, string fileKey = "file",
            IEnumerable<KeyValuePair<string, string>>? formFields = null, IEnumerable<KeyValuePair<string, string>>? headers = null)
            => HttpPostFile(url, Path.GetFileName(filePath), File.ReadAllBytes(filePath), fileKey, formFields, headers);

        /// <summary>
        /// PostFile
        /// </summary>
        /// <param name="url">post url</param>
        /// <param name="fileName">fileName</param>
        /// <param name="fileBytes">fileBytes</param>
        /// <param name="fileKey">fileKey in form,default is "file"</param>
        /// <param name="formFields">other form fields</param>
        /// <param name="headers">request headers</param>
        /// <returns></returns>
        public static string HttpPostFile(string url, string fileName, byte[] fileBytes, string fileKey = "file", IEnumerable<KeyValuePair<string, string>>? formFields = null, IEnumerable<KeyValuePair<string, string>>? headers = null)
        {
            var request = WebRequest.CreateHttp(url);
            var boundary = $"----------------------------{DateTime.Now.Ticks:X}";

            request.ContentType = $"multipart/form-data; boundary={boundary}";
            request.Method = "POST";
            request.KeepAlive = true;

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers[header.Key] = header.Value;
                }
            }

            var boundarybytes = Encoding.ASCII.GetBytes($"\r\n--{boundary}\r\n");
            var endBoundaryBytes = Encoding.ASCII.GetBytes($"\r\n--{boundary}--");

            using (var memStream = new MemoryStream())
            {
                if (formFields != null)
                {
                    foreach (var pair in formFields)
                    {
                        memStream.Write(Encoding.UTF8.GetBytes(string.Format(FormDataFormat, pair.Key, pair.Value, boundary)));
                    }
                }

                memStream.Write(boundarybytes);

                memStream.Write(Encoding.UTF8.GetBytes(string.Format(FileHeaderFormat, fileKey, fileName)));

                memStream.Write(fileBytes);

                memStream.Write(endBoundaryBytes);

                request.ContentLength = memStream.Length;

                using (var requestStream = request.GetRequestStream())
                {
                    memStream.Seek(0, SeekOrigin.Begin);
                    requestStream.Write(memStream.ToArray());
                }

                return request.GetReponseStringSafe();
            }
        }

        /// <summary>
        /// PostMultiFile
        /// </summary>
        /// <param name="url">post url</param>
        /// <param name="filePaths">files</param>
        /// <param name="formFields">other form fields</param>
        /// <param name="headers">request headers</param>
        /// <returns></returns>
        public static string HttpPostFile(string url, IEnumerable<string> filePaths,
            IEnumerable<KeyValuePair<string, string>>? formFields = null, IEnumerable<KeyValuePair<string, string>>? headers = null)
            => HttpPostFile(url,
                filePaths.Select(_ => new KeyValuePair<string, byte[]>(Path.GetFileName(_), File.ReadAllBytes(_))),
                formFields, headers);

        /// <summary>
        /// PostMultiFile
        /// </summary>
        /// <param name="url">post url</param>
        /// <param name="files">files</param>
        /// <param name="formFields">other form fields</param>
        /// <param name="headers">request headers</param>
        /// <returns></returns>
        public static string HttpPostFile(string url, IEnumerable<KeyValuePair<string, byte[]>> files, IEnumerable<KeyValuePair<string, string>>? formFields = null, IEnumerable<KeyValuePair<string, string>>? headers = null)
        {
            var boundary = $"----------------------------{DateTime.Now.Ticks:X}";

            var request = WebRequest.CreateHttp(url);
            request.ContentType = $"multipart/form-data; boundary={boundary}";
            request.Method = "POST";
            request.KeepAlive = true;

            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers[header.Key] = header.Value;
                }
            }

            var boundarybytes = Encoding.ASCII.GetBytes($"\r\n--{boundary}\r\n");
            var endBoundaryBytes = Encoding.ASCII.GetBytes($"\r\n--{boundary}--");

            using (var memStream = new MemoryStream())
            {
                if (formFields != null)
                {
                    foreach (var pair in formFields)
                    {
                        memStream.Write(Encoding.UTF8.GetBytes(string.Format(FormDataFormat, pair.Key, pair.Value, boundary)));
                    }
                }

                foreach (var file in files)
                {
                    memStream.Write(boundarybytes);

                    memStream.Write(Encoding.UTF8.GetBytes(string.Format(FileHeaderFormat, Path.GetFileNameWithoutExtension(file.Key), file.Key)));
                    memStream.Write(file.Value);
                }

                memStream.Write(endBoundaryBytes);
                request.ContentLength = memStream.Length;

                using (var requestStream = request.GetRequestStream())
                {
                    memStream.Seek(0, SeekOrigin.Begin);
                    requestStream.Write(memStream.ToArray());
                }

                return request.GetReponseStringSafe();
            }
        }

        /// <summary>
        /// PostFileAsync
        /// https://stackoverflow.com/questions/566462/upload-files-with-httpwebrequest-multipart-form-data
        /// http://www.cnblogs.com/greenerycn/archive/2010/05/15/csharp_http_post.html
        /// </summary>
        /// <param name="url">post url</param>
        /// <param name="filePath">filePath</param>
        /// <param name="fileKey">fileKey in form,default is "file"</param>
        /// <param name="formFields">other form fields</param>
        /// <param name="headers">request headers</param>
        /// <returns>response text</returns>
        public static Task<string> HttpPostFileAsync(string url, string filePath, string fileKey = "file",
            IEnumerable<KeyValuePair<string, string>>? formFields = null, IEnumerable<KeyValuePair<string, string>>? headers = null)
            => HttpPostFileAsync(url, Path.GetFileName(filePath), File.ReadAllBytes(filePath), fileKey, formFields, headers);

        /// <summary>
        /// PostFileAsync
        /// </summary>
        /// <param name="url">post url</param>
        /// <param name="fileName">fileName</param>
        /// <param name="fileBytes">fileBytes</param>
        /// <param name="fileKey">fileKey in form,default is "file"</param>
        /// <param name="formFields">other form fields</param>
        /// <param name="headers">request headers</param>
        /// <returns></returns>
        public static async Task<string> HttpPostFileAsync(string url, string fileName, byte[] fileBytes, string fileKey = "file", IEnumerable<KeyValuePair<string, string>>? formFields = null, IEnumerable<KeyValuePair<string, string>>? headers = null)
        {
            var boundary = $"----------------------------{DateTime.Now.Ticks:X}";

            var request = WebRequest.CreateHttp(url);
            request.ContentType = $"multipart/form-data; boundary={boundary}";
            request.Method = "POST";
            request.KeepAlive = true;
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers[header.Key] = header.Value;
                }
            }
            var boundaryBytes = Encoding.ASCII.GetBytes($"\r\n--{boundary}\r\n");
            var endBoundaryBytes = Encoding.ASCII.GetBytes($"\r\n--{boundary}--");

            using (var memStream = new MemoryStream())
            {
                if (formFields != null)
                {
                    foreach (var pair in formFields)
                    {
                        memStream.Write(Encoding.UTF8.GetBytes(string.Format(FormDataFormat, pair.Key, pair.Value, boundary)));
                    }
                }

                await memStream.WriteAsync(boundaryBytes);

                await memStream.WriteAsync(Encoding.UTF8.GetBytes(string.Format(FileHeaderFormat, fileKey, fileName)));

                await memStream.WriteAsync(fileBytes);

                await memStream.WriteAsync(endBoundaryBytes);

                request.ContentLength = memStream.Length;

                using (var requestStream = await request.GetRequestStreamAsync())
                {
                    memStream.Seek(0, SeekOrigin.Begin);
                    await requestStream.WriteAsync(memStream.ToArray());
                }

                return await request.GetReponseStringSafeAsync();
            }
        }

        /// <summary>
        /// Post Multi File Async
        /// </summary>
        /// <param name="url">post url</param>
        /// <param name="filePaths">files</param>
        /// <param name="formFields">other form fields</param>
        /// <param name="headers">request headers</param>
        /// <returns></returns>
        public static Task<string> HttpPostFileAsync(string url, IEnumerable<string> filePaths,
            IEnumerable<KeyValuePair<string, string>>? formFields = null, IEnumerable<KeyValuePair<string, string>>? headers = null)
            => HttpPostFileAsync(url,
                filePaths.Select(_ => new KeyValuePair<string, byte[]>(Path.GetFileName(_), File.ReadAllBytes(_))),
                formFields, headers);

        /// <summary>
        /// Post Multi File Async
        /// </summary>
        /// <param name="url">post url</param>
        /// <param name="files">files</param>
        /// <param name="formFields">other form fields</param>
        /// <param name="headers">request headers</param>
        /// <returns></returns>
        public static async Task<string> HttpPostFileAsync(string url, IEnumerable<KeyValuePair<string, byte[]>> files, IEnumerable<KeyValuePair<string, string>>? formFields = null, IEnumerable<KeyValuePair<string, string>>? headers = null)
        {
            var boundary = $"----------------------------{DateTime.Now.Ticks:X}";

            var request = WebRequest.CreateHttp(url);
            request.ContentType = $"multipart/form-data; boundary={boundary}";
            request.Method = "POST";
            request.KeepAlive = true;
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers[header.Key] = header.Value;
                }
            }
            var boundaryBytes = Encoding.ASCII.GetBytes($"\r\n--{boundary}\r\n");
            var endBoundaryBytes = Encoding.ASCII.GetBytes($"\r\n--{boundary}--");

            using (var memStream = new MemoryStream())
            {
                if (formFields != null)
                {
                    foreach (var pair in formFields)
                    {
                        memStream.Write(
                            Encoding.UTF8.GetBytes(string.Format(FormDataFormat, pair.Key, pair.Value, boundary)));
                    }
                }

                foreach (var file in files)
                {
                    await memStream.WriteAsync(boundaryBytes);

                    await memStream.WriteAsync(Encoding.UTF8.GetBytes(
                        string.Format(FileHeaderFormat, Path.GetFileNameWithoutExtension(file.Key), file.Key)));
                    await memStream.WriteAsync(file.Value);
                }

                await memStream.WriteAsync(endBoundaryBytes);

                request.ContentLength = memStream.Length;

                using (var requestStream = await request.GetRequestStreamAsync())
                {
                    memStream.Seek(0, SeekOrigin.Begin);
                    await requestStream.WriteAsync(memStream.ToArray());
                }

                return await request.GetReponseStringSafeAsync();
            }
        }

        #endregion HttpPost

        #endregion WebRequest

        #region UserAgents

        private static readonly string[] MobileUserAgents =
        {
            "Mozilla/5.0 (iPhone 84; CPU iPhone OS 10_3_3 like Mac OS X) AppleWebKit/603.3.8 (KHTML, like Gecko) Version/10.0 MQQBrowser/7.8.0 Mobile/14G60 Safari/8536.25 MttCustomUA/2 QBWebViewType/1 WKType/1",
            "Mozilla/5.0 (Linux; Android 7.0; STF-AL10 Build/HUAWEISTF-AL10; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/53.0.2785.49 Mobile MQQBrowser/6.2 TBS/043508 Safari/537.36 V1_AND_SQ_7.2.0_730_YYB_D QQ/7.2.0.3270 NetType/4G WebP/0.3.0 Pixel/1080",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 10_3_3 like Mac OS X) AppleWebKit/603.3.8 (KHTML, like Gecko) Mobile/14G60 MicroMessenger/6.5.18 NetType/WIFI Language/en",
            "Mozilla/5.0 (Linux; Android 5.1.1; vivo Xplay5A Build/LMY47V; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/48.0.2564.116 Mobile Safari/537.36 T7/9.3 baiduboxapp/9.3.0.10 (Baidu; P1 5.1.1)",
            "Mozilla/5.0 (Linux; U; Android 7.0; zh-cn; STF-AL00 Build/HUAWEISTF-AL00) AppleWebKit/537.36 (KHTML, like Gecko)Version/4.0 Chrome/37.0.0.0 MQQBrowser/7.9 Mobile Safari/537.36",
            "Mozilla/5.0 (iPhone 92; CPU iPhone OS 10_3_2 like Mac OS X) AppleWebKit/603.2.4 (KHTML, like Gecko) Version/10.0 MQQBrowser/7.7.2 Mobile/14F89 Safari/8536.25 MttCustomUA/2 QBWebViewType/1 WKType/1",
            "Mozilla/5.0 (Linux; U; Android 6.0.1; zh-CN; SM-C7000 Build/MMB29M) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/40.0.2214.89 UCBrowser/11.6.2.948 Mobile Safari/537.36",
            "Mozilla/5.0 (Linux; U; Android 5.1.1; zh-cn; MI 4S Build/LMY47V) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/53.0.2785.146 Mobile Safari/537.36 XiaoMi/MiuiBrowser/9.1.3",
            "Mozilla/5.0 (Linux; Android 7.0; MIX Build/NRD90M; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/53.0.2785.49 Mobile MQQBrowser/6.2 TBS/043508 Safari/537.36 V1_AND_SQ_7.2.0_730_YYB_D QQ/7.2.0.3270 NetType/WIFI WebP/0.3.0 Pixel/1080",
            "Mozilla/5.0 (Linux; Android 7.1.1; MI 6 Build/NMF26X; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/53.0.2785.49 Mobile MQQBrowser/6.2 TBS/043508 Safari/537.36 MicroMessenger/6.5.13.1100 NetType/WIFI Language/zh_CN",
            "Mozilla/5.0 (Linux; U; Android 7.0; zh-cn; MIX Build/NRD90M) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/53.0.2785.146 Mobile Safari/537.36 XiaoMi/MiuiBrowser/9.2.2",
            "Mozilla/5.0 (Linux; U; Android 6.0.1; zh-cn; MIX Build/MXB48T) AppleWebKit/537.36 (KHTML, like Gecko)Version/4.0 Chrome/37.0.0.0 MQQBrowser/7.8 Mobile Safari/537.36"
        };

        private static readonly string[] DesktopUserAgents =
        {
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2062.103 Safari/537.36",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_10_2) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/40.0.2214.38 Safari/537.36",
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/46.0.2490.71 Safari/537.36",
            "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/51.0.2704.103 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.62 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/48.0.2564.97 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; WOW64; rv:45.0) Gecko/20100101 Firefox/45.0",
            "Opera/9.80 (Windows NT 6.2; Win64; x64) Presto/2.12.388 Version/12.17",
            "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)",
            "Mozilla/5.0 (Windows NT 6.1) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36 OPR/26.0.1656.60",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:45.0) Gecko/20100101 Firefox/45.0",
            "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/49.0.2623.87 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/50.0.2661.87 Safari/537.36",
            "Mozilla/5.0 (Windows NT 5.2) AppleWebKit/534.30 (KHTML, like Gecko) Chrome/12.0.742.122 Safari/534.30",
            "Mozilla/5.0 (Windows NT 5.1; rv:5.0) Gecko/20100101 Firefox/5.0",
            "Opera/9.80 (Windows NT 6.1; U; zh-cn) Presto/2.9.168 Version/11.50",
            "Mozilla/5.0 (Windows; U; Windows NT 5.1; ) AppleWebKit/534.12 (KHTML, like Gecko) Maxthon/3.0 Safari/534.12"
        };

        private static readonly string[] WechatUserAgents =
        {
            "Mozilla/5.0 (Linux; Android 6.0; 1503-M02 Build/MRA58K) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/37.0.0.0 Mobile MQQBrowser/6.2 TBS/036558 Safari/537.36 MicroMessenger/6.3.25.861 NetType/WIFI Language/zh_CN",
            "Mozilla/5.0 (Linux; Android 5.1; OPPO R9tm Build/LMY47I; wv) AppleWebKit/537.36 (KHTML, like Gecko) Version/4.0 Chrome/53.0.2785.49 Mobile MQQBrowser/6.2 TBS/043220 Safari/537.36 MicroMessenger/6.5.7.1041 NetType/4G Language/zh_CN",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 9_3 like Mac OS X) AppleWebKit/601.1.46 (KHTML, like Gecko) Mobile/13E233 MicroMessenger/6.3.15 NetType/WIFI Language/zh_CN",
            "Mozilla/5.0 (iPhone; CPU iPhone OS 10_2_1 like Mac OS X) AppleWebKit/602.4.6 (KHTML, like Gecko) Mobile/14D27 MicroMessenger/6.5.6 NetType/4G Language/zh_CN"
        };

        /// <summary>
        /// GetUserAgent
        /// </summary>
        /// <param name="isMobileUserAgent">isMobileUserAgent</param>
        /// <returns>UserAgent</returns>
        public static string GetUserAgent(bool isMobileUserAgent = false)
        {
            return isMobileUserAgent ? MobileUserAgents[SecurityHelper.Random.Next(MobileUserAgents.Length)] : DesktopUserAgents[SecurityHelper.Random.Next(DesktopUserAgents.Length)];
        }

        /// <summary>
        /// GetWechatUserAgent
        /// </summary>
        /// <returns></returns>
        public static string GetWechatUserAgent()
            => WechatUserAgents[SecurityHelper.Random.Next(WechatUserAgents.Length)];

        #endregion UserAgents
    }
}
