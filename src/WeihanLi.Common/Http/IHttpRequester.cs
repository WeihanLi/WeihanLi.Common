using System.Net;

namespace WeihanLi.Common.Http;

public interface IHttpRequester
{
    #region WithUrl

    IHttpRequester WithUrl(string url);

    #endregion WithUrl

    #region Method

    IHttpRequester WithMethod(HttpMethod method);

    #endregion Method

    #region AddHeader

    IHttpRequester WithHeaders(IEnumerable<KeyValuePair<string, string?>> customHeaders);

    #endregion AddHeader

    #region UserAgent

    IHttpRequester WithUserAgent(string userAgent);

    #endregion UserAgent

    #region Referer

    IHttpRequester WithReferer(string referer);

    #endregion Referer

    #region Cookie

    IHttpRequester WithCookie(string? url, Cookie cookie);

    IHttpRequester WithCookie(string? url, CookieCollection cookies);

    #endregion Cookie

    #region Proxy

    IHttpRequester WithProxy(IWebProxy proxy);

    #endregion Proxy

    #region Parameter

    IHttpRequester WithParameters(byte[] requestBytes, string contentType);

    IHttpRequester WithFile(string fileName, byte[] fileBytes, string fileKey = "file",
        IEnumerable<KeyValuePair<string, string>>? formFields = null);

    IHttpRequester WithFiles(IEnumerable<KeyValuePair<string, byte[]>> files,
        IEnumerable<KeyValuePair<string, string>>? formFields = null);

    #endregion Parameter

    #region Execute

    byte[] ExecuteBytes();

    Task<byte[]> ExecuteBytesAsync();

    HttpResponse ExecuteForResponse();

    Task<HttpResponse> ExecuteForResponseAsync();

    #endregion Execute
}
