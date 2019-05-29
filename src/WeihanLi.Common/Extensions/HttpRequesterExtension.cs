using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Extensions
{
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
                new KeyValuePair<string, string>("X-Requested-With", "XMLHttpRequest")
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

        public static IHttpRequester WithFile(this IHttpRequester httpRequester, string filePath, string fileKey = "file",
            IEnumerable<KeyValuePair<string, string>> formFields = null)
            => httpRequester.WithFile(Path.GetFileName(filePath), File.ReadAllBytes(filePath), fileKey, formFields);

        public static IHttpRequester WithFiles(this IHttpRequester httpRequester, IEnumerable<string> filePaths, IEnumerable<KeyValuePair<string, string>> formFields = null)
            => httpRequester.WithFiles(
                filePaths.Select(_ => new KeyValuePair<string, byte[]>(
                    Path.GetFileName(_),
                    File.ReadAllBytes(_))),
                formFields);
    }
}
