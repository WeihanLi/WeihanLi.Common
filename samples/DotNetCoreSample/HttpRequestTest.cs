using System.Collections.Generic;
using System.Net.Http;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace DotNetCoreSample
{
    internal class HttpRequestTest
    {
        public static void MainTest()
        {
            var request = new HttpRequest("https://weihanli.xyz")
                .WithReferer("https://weihanli.xyz")
                .WithHeaders(new System.Collections.Specialized.NameValueCollection
                {
                    {"Header1", "Header1" }
                });
            var result = request.Execute();
            System.Console.WriteLine(result);

            var loginResult = new HttpRequest("https://accounting.weihanli.xyz/Account/LogOn", HttpMethod.Post)
                .WithHeaders(new Dictionary<string, string>()
                {
                    { "X-Requested-With", "XMLHttpRequest" },
                })
                // .AjaxRequest(true)
                .WithReferer("https://accounting.weihanli.xyz/Account/Login?ReturnUrl=%2F")
                .WithUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36")
                .WithFormParameters(new Dictionary<string, string>()
                {
                    {"Username","liweihan" },
                    {"Password", "112233" },
                    {"RememberMe","false" }
                })
                .Execute<WeihanLi.Common.Models.JsonResultModel<bool>>();
            System.Console.WriteLine(loginResult.ToJson());

            var uploadFileResponse = new HttpRequest("https://graph.baidu.com/upload", HttpMethod.Post)
                .WithFile($@"{System.Environment.GetEnvironmentVariable("USERPROFILE")}\Pictures\4e6ab53e383863ed4d15252039f70423.jpg", "image", new Dictionary<string, string>()
                {
                    { "tn","pc" },
                    { "from","pc" },
                    { "image_source","PC_UPLOAD_SEARCH_FILE" },
                    { "range","{\"page_from\": \"searchIndex\"}" },
                })
                .WithReferer("https://baidu.com/")
                .WithUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/72.0.3626.121 Safari/537.36")
                .ExecuteForResponse();
            System.Console.WriteLine($"Response status:{uploadFileResponse.StatusCode}, result:{uploadFileResponse.ReadToEnd()}");
        }
    }
}
