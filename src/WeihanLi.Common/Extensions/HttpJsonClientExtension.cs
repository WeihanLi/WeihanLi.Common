using Newtonsoft.Json;
using WeihanLi.Common;
using WeihanLi.Common.Http;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class HttpJsonClientExtension
{
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
}
