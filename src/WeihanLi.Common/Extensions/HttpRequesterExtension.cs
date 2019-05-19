using System.Collections.Generic;
using System.IO;
using System.Linq;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Extensions
{
    public static class HttpRequesterExtension
    {
        public static IHttpRequester AjaxRequest(this IHttpRequester httpRequester)
        {
            return httpRequester.WithHeaders(new[]
            {
                new KeyValuePair<string, string>("X-Requested-With", "XMLHttpRequest")
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

        //public static Task<HttpResponseMessage> ExecuteForResponseAsync(this IHttpRequester httpRequester)
        //{
        //}
    }
}
