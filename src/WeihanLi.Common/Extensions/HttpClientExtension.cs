﻿using Newtonsoft.Json;
using System.Net.Http.Headers;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Http;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class HttpClientExtension
{
    /// <summary>
    /// HTTP Basic Authentication authorization header for RFC6749 client authentication
    /// </summary>
    /// <seealso cref="T:System.Net.Http.Headers.AuthenticationHeaderValue" />
    /// <remarks>
    /// Initializes a new instance of the <see cref="T:System.Net.Http.BasicAuthenticationOAuthHeaderValue" /> class.
    /// </remarks>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    private sealed class BasicAuthenticationHeaderValue(string userName, string password) : AuthenticationHeaderValue("Basic", EncodeCredential(userName, password))
    {
        private static string EncodeCredential(string userName, string password)
        {
            Guard.NotNullOrWhiteSpace(userName);
            return Convert.ToBase64String($"{UrlEncode(userName)}:{UrlEncode(password)}".GetBytes());
        }

        private static string UrlEncode(string value)
        {
            return string.IsNullOrEmpty(value)
                ? string.Empty
                : Uri.EscapeDataString(value).Replace("%20", "+");
        }
    }

    /// <summary>
    /// Post object as json request body
    /// </summary>
    public static Task<HttpResponseMessage> PostJsonRequestAsync<T>(this HttpClient httpClient, string requestUrl, T parameter, Action<HttpRequestMessage>? requestAction = null,
        CancellationToken cancellationToken = default)
        => HttpJsonRequestAsync(httpClient, HttpMethod.Post, requestUrl, parameter, requestAction, cancellationToken);

    /// <summary>
    /// PutAsJsonAsync
    /// </summary>
    public static Task<HttpResponseMessage> PutJsonRequestAsync<T>(this HttpClient httpClient, string requestUrl, T parameter, Action<HttpRequestMessage>? requestAction = null,
        CancellationToken cancellationToken = default)
        => HttpJsonRequestAsync(httpClient, HttpMethod.Put, requestUrl, parameter, requestAction, cancellationToken);

    /// <summary>
    /// PostJson request body and get object from json response
    /// </summary>
    public static Task<TResponse?> PostJsonAsync<TRequest, TResponse>
    (this HttpClient httpClient, string requestUrl,
        TRequest request, Action<HttpRequestMessage>? requestAction = null,
        Action<HttpResponseMessage>? responseAction = null,
        CancellationToken cancellationToken = default)
        => HttpJsonAsync<TRequest, TResponse>(httpClient, HttpMethod.Post, requestUrl, request, requestAction, responseAction,
            cancellationToken);

    /// <summary>
    /// Put Json request body and get object from json response
    /// </summary>
    public static Task<TResponse?> PutJsonAsync<TRequest, TResponse>
    (this HttpClient httpClient, string requestUrl,
        TRequest request,
        Action<HttpRequestMessage>? requestAction = null,
        Action<HttpResponseMessage>? responseAction = null,
        CancellationToken cancellationToken = default)
        => HttpJsonAsync<TRequest, TResponse>(httpClient, HttpMethod.Put, requestUrl, request, requestAction, responseAction,
            cancellationToken);

    public static async Task<HttpResponseMessage> HttpJsonRequestAsync<TRequest>
    (this HttpClient httpClient, HttpMethod httpMethod, string requestUrl,
        TRequest request, Action<HttpRequestMessage>? requestAction = null,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(httpClient);
        using var requestMessage = new HttpRequestMessage(httpMethod, requestUrl);
        requestMessage.Content = JsonHttpContent.From(request);
        requestAction?.Invoke(requestMessage);
        return await httpClient.SendAsync(requestMessage, cancellationToken);
    }

    public static async Task<TResponse?> ReadJsonResponseAsync<TResponse>
    (this HttpResponseMessage response, Action<HttpResponseMessage>? responseAction = null,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(response);
        responseAction?.Invoke(response);
#if NET
        var responseText = await response.Content.ReadAsStringAsync(cancellationToken);
#else
        var responseText = await response.Content.ReadAsStringAsync();
#endif
        return JsonConvert.DeserializeObject<TResponse>(responseText);
    }

    public static async Task<TResponse?> HttpJsonAsync<TRequest, TResponse>
    (this HttpClient httpClient, HttpMethod httpMethod, string requestUrl,
        TRequest request, Action<HttpRequestMessage>? requestAction = null,
        Action<HttpResponseMessage>? responseAction = null,
        CancellationToken cancellationToken = default)
    {
        Guard.NotNull(httpClient);
        using var requestMessage = new HttpRequestMessage(httpMethod, requestUrl);
        requestMessage.Content = JsonHttpContent.From(request);
        requestAction?.Invoke(requestMessage);
        using var response = await httpClient.SendAsync(requestMessage, cancellationToken);
        responseAction?.Invoke(response);
#if NET
        var responseText = await response.Content.ReadAsStringAsync(cancellationToken);
#else
        var responseText = await response.Content.ReadAsStringAsync();
#endif
        return JsonConvert.DeserializeObject<TResponse>(responseText);
    }

#if NET
    /// <summary>
    /// PatchAsJsonAsync
    /// </summary>
    public static Task<HttpResponseMessage> PatchJsonRequestAsync<T>(this HttpClient httpClient, string requestUrl, T parameter, Action<HttpRequestMessage>? requestAction = null,
        CancellationToken cancellationToken = default)
         => HttpJsonRequestAsync(httpClient, HttpMethod.Patch, requestUrl, parameter, requestAction, cancellationToken);

    /// <summary>
    /// Patch Json request body and get object from json response
    /// </summary>
    public static Task<TResponse?> PatchJsonAsync<TRequest, TResponse>
    (this HttpClient httpClient, string requestUrl,
        TRequest request, Action<HttpRequestMessage>? requestAction = null,
        Action<HttpResponseMessage>? responseAction = null,
        CancellationToken cancellationToken = default)
        => HttpJsonAsync<TRequest, TResponse>(httpClient, HttpMethod.Patch, requestUrl, request, requestAction, responseAction,
            cancellationToken);
#endif

    /// <summary>
    /// PostAsFormAsync
    /// </summary>
    public static Task<HttpResponseMessage> PostAsFormAsync(this HttpClient httpClient, string requestUri, IEnumerable<KeyValuePair<string, string>> paramDic, CancellationToken cancellationToken = default)
        => httpClient.PostAsync(requestUri, new FormUrlEncodedContent(paramDic), cancellationToken);

    /// <summary>
    /// PutAsFormAsync
    /// </summary>
    public static Task<HttpResponseMessage> PutAsFormAsync(this HttpClient httpClient, string requestUri, IEnumerable<KeyValuePair<string, string>> paramDic, CancellationToken cancellationToken = default)
        => httpClient.PutAsync(requestUri, new FormUrlEncodedContent(paramDic), cancellationToken);

    /// <summary>
    /// HttpClient Post a file
    /// </summary>
    /// <param name="httpClient">httpClient</param>
    /// <param name="requestUrl">requestUrl</param>
    /// <param name="filePath">filePath</param>
    /// <param name="fileKey">fileKey,default is "file"</param>
    /// <param name="formFields">formFields,default is null</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns>response message</returns>
    public static Task<HttpResponseMessage> PostFileAsync(this HttpClient httpClient,
        string requestUrl,
        string filePath,
        string fileKey = "file",
        IEnumerable<KeyValuePair<string, string>>? formFields = null, CancellationToken cancellationToken = default)
    {
        var content = new MultipartFormDataContent($"form--{DateTime.UtcNow.Ticks:X}");

        if (formFields != null)
        {
            foreach (var kv in formFields)
            {
                content.Add(new StringContent(kv.Value), kv.Key);
            }
        }

        content.Add(new StreamContent(File.OpenRead(filePath)), fileKey, Path.GetFileName(filePath));

        return httpClient.PostAsync(requestUrl, content, cancellationToken);
    }

    /// <summary>
    /// HttpClient Post a file
    /// </summary>
    /// <param name="httpClient">httpClient</param>
    /// <param name="requestUrl">requestUrl</param>
    /// <param name="file">fileStream</param>
    /// <param name="fileName">fileName</param>
    /// <param name="fileKey">fileKey,default is "file"</param>
    /// <param name="formFields">formFields,default is null</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns></returns>
    public static async Task<HttpResponseMessage> PostFileAsync(this HttpClient httpClient,
        string requestUrl,
        Stream? file,
        string fileName,
        string fileKey = "file",
        IEnumerable<KeyValuePair<string, string>>? formFields = null, CancellationToken cancellationToken = default)
    {
        if (file == null)
        {
            return await httpClient.PostAsFormAsync(requestUrl, formFields ?? Array.Empty<KeyValuePair<string, string>>(), cancellationToken);
        }

        var content = new MultipartFormDataContent($"form--{DateTime.UtcNow.Ticks:X}");

        if (formFields != null)
        {
            foreach (var kv in formFields)
            {
                content.Add(new StringContent(kv.Value), kv.Key);
            }
        }

        content.Add(new StreamContent(file), fileKey, fileName);

        return await httpClient.PostAsync(requestUrl, content, cancellationToken);
    }

    public static Task<HttpResponseMessage> PostFileAsync(this HttpClient
            client, string requestUrl, ICollection<string> filePaths,
        IEnumerable<KeyValuePair<string, string>>? formFields = null, CancellationToken cancellationToken = default) =>
        client.PostFileAsync(requestUrl, filePaths.Select(p =>
                new KeyValuePair<string, Stream>(Path.GetFileName(p), File.OpenRead(p))), formFields, cancellationToken);

    /// <summary>
    /// HttpClient Post files. <see href="https://stackoverflow.com/questions/18059588/httpclient-multipart-form-post-in-c-sharp">See here</see>.
    /// </summary>
    /// <param name="httpClient">The HTTP client.</param>
    /// <param name="requestUri">The request URI.</param>
    /// <param name="files">The files. Key: File name, value: file read stream.</param>
    /// <param name="formFields">The form.</param>
    /// <param name="cancellationToken">cancellationToken</param>
    /// <returns></returns>
    public static async Task<HttpResponseMessage> PostFileAsync(this HttpClient httpClient,
        string requestUri,
        IEnumerable<KeyValuePair<string, Stream>>? files,
        IEnumerable<KeyValuePair<string, string>>? formFields = null,
        CancellationToken cancellationToken = default)
    {
        if (files is null)
        {
            return await httpClient.PostAsFormAsync(requestUri, formFields ?? Array.Empty<KeyValuePair<string, string>>(), cancellationToken);
        }

        var content = new MultipartFormDataContent($"form--{DateTime.UtcNow.Ticks:X}");

        if (formFields != null)
        {
            foreach (var kv in formFields)
            {
                content.Add(new StringContent(kv.Value), kv.Key);
            }
        }

        foreach (var file in files)
        {
            content.Add(new StreamContent(file.Value), Path.GetFileNameWithoutExtension(file.Key), Path.GetFileName(file.Key));
        }

        return await httpClient.PostAsync(requestUri, content, cancellationToken);
    }

    /// <summary>
    /// Sets the basic authentication.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    public static void SetBasicAuthentication(this HttpClient client, string userName, string password) =>
        client.DefaultRequestHeaders.Authorization = new BasicAuthenticationHeaderValue(userName, password);

    /// <summary>
    /// Sets a basic authentication header.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    public static void SetBasicAuthentication(this HttpRequestMessage request, string userName, string password) => request.Headers.Authorization = new BasicAuthenticationHeaderValue(userName, password);

    /// <summary>
    /// Sets a basic authentication header for RFC6749 client authentication.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="userName">Name of the user.</param>
    /// <param name="password">The password.</param>
    public static void SetBasicAuthenticationOAuth(this HttpRequestMessage request, string userName, string password) => request.Headers.Authorization = new BasicAuthenticationHeaderValue(userName, password);

    /// <summary>
    /// Sets the token.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="scheme">The scheme.</param>
    /// <param name="token">The token.</param>
    public static void SetToken(this HttpClient client, string scheme, string token) =>
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme, token);

    /// <summary>
    /// Sets an authorization header with a given scheme and value.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="scheme">The scheme.</param>
    /// <param name="token">The token.</param>
    public static void SetToken(this HttpRequestMessage request, string scheme, string token) => request.Headers.Authorization = new AuthenticationHeaderValue(scheme, token);

    /// <summary>
    /// Sets the bearer token.
    /// </summary>
    /// <param name="client">The client.</param>
    /// <param name="token">The token.</param>
    public static void SetBearerToken(this HttpClient client, string token) => client.SetToken("Bearer", token);

    /// <summary>
    /// Sets an authorization header with a bearer token.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="token">The token.</param>
    public static void SetBearerToken(this HttpRequestMessage request, string token) => request.SetToken("Bearer", token);

    /// <summary>
    /// Try to add a header to the request message, would add to content header if the header belongs to content header 
    /// </summary>
    /// <param name="requestMessage">request message to add</param>
    /// <param name="headerName">header name</param>
    /// <param name="headerValue">header value</param>
    /// <returns>the request message</returns>
    public static HttpRequestMessage TryAddHeader(this HttpRequestMessage requestMessage,
        string headerName, string headerValue)
    {
        Guard.NotNull(requestMessage);
        Guard.NotNullOrEmpty(headerName);
        if (HttpHelper.IsWellKnownContentHeader(headerName))
        {
            requestMessage.Content?.Headers.Remove(headerName);
            requestMessage.Content?.Headers.TryAddWithoutValidation(headerName, headerValue);
        }
        else
        {
            requestMessage.Headers.TryAddWithoutValidation(headerName, headerValue);
        }
        return requestMessage;
    }

    /// <summary>
    /// Try to add a header to the request message when not exists, would add to content header if the header belongs to content header 
    /// </summary>
    /// <param name="requestMessage">request message to add</param>
    /// <param name="headerName">header name</param>
    /// <param name="headerValue">header value</param>
    /// <returns>the request message</returns>
    public static HttpRequestMessage TryAddHeaderIfNotExists(this HttpRequestMessage requestMessage,
        string headerName, string headerValue)
    {
        Guard.NotNull(requestMessage);
        Guard.NotNullOrEmpty(headerName);
        if (HttpHelper.IsWellKnownContentHeader(headerName))
        {
            if (requestMessage.Content is not null && !requestMessage.Content.Headers.Contains(headerName))
            {
                requestMessage.Content.Headers.TryAddWithoutValidation(headerName, headerValue);
            }
        }
        else
        {
            if (!requestMessage.Headers.Contains(headerName))
            {
                requestMessage.Headers.TryAddWithoutValidation(headerName, headerValue);
            }
        }
        return requestMessage;
    }
}
