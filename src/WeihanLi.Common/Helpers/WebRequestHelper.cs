// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Net;
using System.Text;
using WeihanLi.Common.Http;
using WeihanLi.Extensions;
using static WeihanLi.Common.Helpers.HttpHelper;

namespace WeihanLi.Common.Helpers;

/// <summary>
/// Http request helper
/// </summary>
public static class WebRequestHelper
{
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
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.Referer))
                {
                    request.Referer = header.Value;
                    continue;
                }
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.UserAgent))
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
        return request.GetResponseString();
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
        if (proxy is null)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            foreach (var header in customHeaders ?? [])
            {
                requestMessage.TryAddHeader(header.Key, header.Value);
            }
            using var responseMessage = await HttpHelper.HttpClient.SendAsync(requestMessage);
            return await responseMessage.Content.ReadAsStringAsync();
        }

        var request = WebRequest.CreateHttp(url);
        request.UserAgent = GetUserAgent();
        request.Method = "GET";
        if (null != customHeaders)
        {
            foreach (var header in customHeaders)
            {
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.Referer))
                {
                    request.Referer = header.Value;
                    continue;
                }
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.UserAgent))
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
        return await request.GetResponseStringSafeAsync();
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
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.Referer))
                {
                    request.Referer = header.Value;
                    continue;
                }
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.UserAgent))
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

        return request.GetResponseBytesSafe();
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
        if (proxy is null)
        {
            using var requestMessage = new HttpRequestMessage(HttpMethod.Get, url);
            foreach (var header in customHeaders ?? [])
            {
                requestMessage.TryAddHeader(header.Key, header.Value);
            }
            using var responseMessage = await HttpHelper.HttpClient.SendAsync(requestMessage);
            return await responseMessage.Content.ReadAsByteArrayAsync();
        }

        var request = WebRequest.CreateHttp(url);
        request.UserAgent = GetUserAgent();
        request.Method = "GET";

        if (null != customHeaders)
        {
            foreach (var header in customHeaders)
            {
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.Referer))
                {
                    request.Referer = header.Value;
                    continue;
                }
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.UserAgent))
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

        return await request.GetResponseBytesSafeAsync();
    }

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T HttpGetFor<T>(string url)
        => HttpGetString(url).StringToType<T>();

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T HttpGetFor<T>(string url, IEnumerable<KeyValuePair<string, string>>? customHeaders)
        => HttpGetString(url, customHeaders).StringToType<T>();

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T HttpGetFor<T>(string url, IEnumerable<KeyValuePair<string, string>>? customHeaders,
        WebProxy? proxy)
        => HttpGetString(url, customHeaders, proxy).StringToType<T>();

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static Task<T> HttpGetForAsync<T>(string url)
        => HttpGetStringAsync(url).ContinueWith(result => result.Result.StringToType<T>());

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static Task<T> HttpGetForAsync<T>(string url, IEnumerable<KeyValuePair<string, string>>? customHeaders)
        => HttpGetStringAsync(url, customHeaders).ContinueWith(result => result.Result.StringToType<T>());

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
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
        if (parameters is { Count: > 0 })
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
        if (parameters is { Count: > 0 })
        {
            url = url + (url.IndexOf('?') < 0 ? "?" : "&") + string.Join("&", parameters.Select(p => $"{WebUtility.UrlEncode(p.Key)}={WebUtility.UrlEncode(p.Value)}"));
        }
        return await HttpGetStringAsync(url);
    }

    public static byte[] HttpGetForBytes(string url, IDictionary<string, string>? parameters)
    {
        if (parameters is { Count: > 0 })
        {
            url = url + (url.IndexOf('?') < 0 ? "?" : "&") + string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
        }
        return HttpGetForBytes(url);
    }

    public static async Task<byte[]> HttpGetForBytesAsync(string url, IDictionary<string, string>? parameters)
    {
        if (parameters is { Count: > 0 })
        {
            url = url + (url.IndexOf('?') < 0 ? "?" : "&") + string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
        }
        return await HttpGetForBytesAsync(url);
    }

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T HttpGetFor<T>(string url, IDictionary<string, string>? parameters)
    {
        if (parameters.HasValue())
        {
            url = url + (url.IndexOf('?') < 0 ? "?" : "&") + string.Join("&", parameters.Select(p => $"{p.Key}={p.Value}"));
        }
        return HttpGetFor<T>(url);
    }

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static async Task<T> HttpGetForAsync<T>(string url, IDictionary<string, string>? parameters)
    {
        if (parameters.HasValue())
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
    private static string GetContentType(bool isJsonFormat) => isJsonFormat ? ApplicationJsonContentType : FormDataContentType;

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

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static TResponse HttpPostJsonFor<TRequest, TResponse>(string url, TRequest data)
    => HttpPostFor<TResponse>(url, Encoding.UTF8.GetBytes(data.ToJson()), true);

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static TResponse HttpPostJsonFor<TRequest, TResponse>(string url, TRequest data, Encoding encoding)
    => HttpPostFor<TResponse>(url, encoding.GetBytes(data.ToJson()), true);

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static Task<TResponse> HttpPostJsonForAsync<TRequest, TResponse>(string url, TRequest data)
        => HttpPostForAsync<TResponse>(url, Encoding.UTF8.GetBytes(data.ToJson()), true);

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
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
        return request.GetResponseStringSafe();
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
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.Referer))
                {
                    request.Referer = header.Value;
                    continue;
                }
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.UserAgent))
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
        return await request.GetResponseStringSafeAsync();
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
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.Referer))
                {
                    request.Referer = header.Value;
                    continue;
                }
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.UserAgent))
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
        return await request.GetResponseStringSafeAsync();
    }

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T HttpPostFor<T>(string url, byte[] postData, bool isJsonFormat)
        => HttpPost(url, postData, isJsonFormat).StringToType<T>();

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
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
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.Referer))
                {
                    request.Referer = header.Value;
                    continue;
                }
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.UserAgent))
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

        return request.GetResponseBytesSafe();
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
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.Referer))
                {
                    request.Referer = header.Value;
                    continue;
                }
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.UserAgent))
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
        return request.GetResponseBytesSafe();
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
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.Referer))
                {
                    request.Referer = header.Value;
                    continue;
                }
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.UserAgent))
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
        return await request.GetResponseBytesSafeAsync();
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
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.Referer))
                {
                    request.Referer = header.Value;
                    continue;
                }
                if (header.Key.EqualsIgnoreCase(HttpHeaderNames.UserAgent))
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
        return await request.GetResponseBytesSafeAsync();
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
        var boundary = $"----------------------------{DateTime.UtcNow.Ticks:X}";

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

        using var memStream = new MemoryStream();
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

        return request.GetResponseStringSafe();
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
        var boundary = $"----------------------------{DateTime.UtcNow.Ticks:X}";

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

        using var memStream = new MemoryStream();
        if (formFields != null)
        {
            foreach (var pair in formFields)
            {
                memStream.Write(Encoding.UTF8.GetBytes(string.Format(FormDataFormat, pair.Key, pair.Value, boundary)));
            }
        }

        foreach (var file in files)
        {
            memStream.Write(boundaryBytes);

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

        return request.GetResponseStringSafe();
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
        var boundary = $"----------------------------{DateTime.UtcNow.Ticks:X}";

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

        using var memStream = new MemoryStream();
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

        return await request.GetResponseStringSafeAsync();
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
        var boundary = $"----------------------------{DateTime.UtcNow.Ticks:X}";

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

        using var memStream = new MemoryStream();
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

        using var requestStream = await request.GetRequestStreamAsync();
        memStream.Seek(0, SeekOrigin.Begin);
        await requestStream.WriteAsync(memStream.ToArray());

        return await request.GetResponseStringSafeAsync();
    }

    #endregion HttpPost

    #endregion WebRequest
}
