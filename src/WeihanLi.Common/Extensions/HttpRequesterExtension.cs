using System.Diagnostics.CodeAnalysis;
using System.Net;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Http;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class HttpRequesterExtension
{
    public static IHttpRequester WithUrl(this IHttpRequester httpRequester, string url, IEnumerable<KeyValuePair<string, string>> queryParams)
    {
        var requestUrl = url.IndexOf("?", StringComparison.OrdinalIgnoreCase) > 0
            ? $"{url}?{queryParams.ToQueryString()}"
            : $"{url}&{queryParams.ToQueryString()}";
        return httpRequester.WithUrl($"{requestUrl}");
    }

    public static IHttpRequester AjaxRequest(this IHttpRequester httpRequester)
    {
        return httpRequester.WithHeaders(new[]
        {
                new KeyValuePair<string, string?>("X-Requested-With", "XMLHttpRequest")
            });
    }

    public static IHttpRequester WithCookie(this IHttpRequester httpRequester, string cookieName, string cookieValue)
    {
        return httpRequester.WithCookie(new Cookie(cookieName, cookieValue));
    }

    public static IHttpRequester WithCookie(this IHttpRequester httpRequester, Cookie cookie)
    {
        return httpRequester.WithCookie(null, cookie);
    }

    public static IHttpRequester WithCookie(this IHttpRequester httpRequester, CookieCollection cookies)
    {
        return httpRequester.WithCookie(null, cookies);
    }

    public static IHttpRequester WithProxy(this IHttpRequester httpRequester, string url)
    {
        return httpRequester.WithProxy(new WebProxy(new Uri(url)));
    }

    public static IHttpRequester WithProxy(this IHttpRequester httpRequester, string url, string userName, string password)
    {
        return httpRequester.WithProxy(new WebProxy(new Uri(url))
        {
            Credentials = new NetworkCredential(userName, password)
        });
    }

    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    public static IHttpRequester WithXmlParameter<TEntity>(this IHttpRequester httpRequester, TEntity entity)
    {
        return httpRequester.WithParameters(XmlDataSerializer.Instance.Value.Serialize(entity), "application/xml;charset=UTF-8");
    }

    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    public static IHttpRequester WithJsonParameter<TEntity>(this IHttpRequester httpRequester, TEntity entity)
    {
        return httpRequester.WithParameters(entity.ToJson().GetBytes(), HttpHelper.ApplicationJsonContentType);
    }

    public static IHttpRequester WithFormParams(this IHttpRequester httpRequester,
        IEnumerable<KeyValuePair<string, string>> formParams)
    {
        return httpRequester.WithParameters(formParams.ToQueryString().GetBytes(), HttpHelper.FormDataContentType);
    }

    public static IHttpRequester WithFile(this IHttpRequester httpRequester, string filePath, string fileKey = "file",
        IEnumerable<KeyValuePair<string, string>>? formFields = null)
        => httpRequester.WithFile(Path.GetFileName(filePath), File.ReadAllBytes(filePath), fileKey, formFields);

    public static IHttpRequester WithFiles(this IHttpRequester httpRequester, IEnumerable<string> filePaths, IEnumerable<KeyValuePair<string, string>>? formFields = null)
        => httpRequester.WithFiles(
            filePaths.Select(f => new KeyValuePair<string, byte[]>(
                Path.GetFileName(f),
                File.ReadAllBytes(f))),
            formFields);

    public static string Execute(this IHttpRequester httpRequester) => httpRequester.ExecuteBytes().GetString();

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static T? Execute<T>(this IHttpRequester httpRequester, T? defaultVal = default) => httpRequester.ExecuteBytes().GetString().ToOrDefault(defaultVal);

    public static Task<string> ExecuteAsync(this IHttpRequester httpRequester)
    {
        return httpRequester.ExecuteBytesAsync().ContinueWith(r => r.Result.GetString());
    }

    [RequiresUnreferencedCode("Generic TypeConverters may require the generic types to be annotated. For example, NullableConverter requires the underlying type to be DynamicallyAccessedMembers All.")]
    public static Task<T?> ExecuteAsync<T>(this IHttpRequester httpRequester, T? defaultVal = default)
    {
        return httpRequester.ExecuteBytesAsync().ContinueWith(r => r.Result.GetString().ToOrDefault(defaultVal));
    }

    public static TEntity ExecuteForJson<TEntity>(this IHttpRequester httpRequester)
    {
        return httpRequester.Execute().JsonToObject<TEntity>();
    }

    public static Task<TEntity> ExecuteForJsonAsync<TEntity>(this IHttpRequester httpRequester)
    {
        return httpRequester.ExecuteAsync().ContinueWith(r => r.Result.JsonToObject<TEntity>());
    }

    [RequiresDynamicCode("XML serializer relies on dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    public static TEntity ExecuteForXml<TEntity>(this IHttpRequester httpRequester)
    {
        return XmlDataSerializer.Instance.Value.Deserialize<TEntity>(httpRequester.ExecuteBytes());
    }

    [RequiresDynamicCode("XML serializer relies on dynamic code generation which is not available with Ahead of Time compilation.")]
    [RequiresUnreferencedCode("Members from serialized types may be trimmed if not referenced directly")]
    public static Task<TEntity> ExecuteForXmlAsync<TEntity>(this IHttpRequester httpRequester)
    {
        return httpRequester.ExecuteBytesAsync().ContinueWith(r => XmlDataSerializer.Instance.Value.Deserialize<TEntity>(r.Result));
    }
}
