using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace DotNetFxSample
{
    public class HttpHelperSample
    {
        private const string PostUploadFileUrl = "http://localhost:64984/Home/Upload";

        public static void GetRequest()
        {
            var response = HttpHelper.HttpGetString("https://baidu.com");
            Console.WriteLine(response);
        }

        public static void PostRequest()
        {
            var byteArray = Encoding.UTF8.GetBytes("UserName=111&Password=222&RememberMe=false");
            var response = HttpHelper.HttpPost("https://accounting.weihanli.xyz/Account/Logon", byteArray, false);
            Console.WriteLine(response);
            //
            var paramDic = new Dictionary<string, string>
            {
                { "UserName", "1234" },
                { "Password", "123444"},
                {"RememberMe","false" }
            };
            response = HttpHelper.HttpPost("https://accounting.weihanli.xyz/Account/Logon", paramDic);
            Console.WriteLine(response);
        }

        public static async Task HttpPostFile()
        {
            var response = HttpHelper.HttpPostFile(PostUploadFileUrl,
                @"C:\Users\liweihan.TUHU\Pictures\temp\7604648.jpg");
            Console.WriteLine(response);

            // more parameters
            response = HttpHelper.HttpPostFile(PostUploadFileUrl,
                @"C:\Users\liweihan.TUHU\Pictures\temp\7604648.jpg",
                fileKey: "image",
                formFields: new NameValueCollection
                {
                    {"user","abc" },
                    {"pwd","567" }
                }.ToDictionary());
            Console.WriteLine(response);

            //Upload directly by bytes
            //response = HttpHelper.HttpPostFile(PostUploadFileUrl,
            //    "abc.jpg", File.ReadAllBytes(@"C:\Users\liweihan.TUHU\Pictures\temp\7604648.jpg"));
            //Console.WriteLine(response);

            response = await HttpHelper.HttpPostFileAsync(PostUploadFileUrl,
                @"C:\Users\liweihan.TUHU\Pictures\temp\7604648.jpg");
            Console.WriteLine(response);

            response = await HttpHelper.HttpPostFileAsync(PostUploadFileUrl,
                @"C:\Users\liweihan.TUHU\Pictures\temp\7604648.jpg",
                fileKey: "image",
                formFields: new Dictionary<string, string>
                {
                    {"user","abc" },
                    {"pwd","567" }
                });
            Console.WriteLine(response);

            //response = await HttpHelper.HttpPostFileAsync(PostUploadFileUrl,
            //    "abc.jpg", File.ReadAllBytes(@"C:\Users\liweihan.TUHU\Pictures\temp\7604648.jpg"));
            //Console.WriteLine(response);

            response = HttpHelper.HttpPostFile(PostUploadFileUrl,
                new[]
                {
                    @"C:\Users\liweihan.TUHU\Pictures\temp\icon.png",
                    @"C:\Users\liweihan.TUHU\Pictures\temp\7604648.jpg",
                });
            Console.WriteLine(response);

            response = await HttpHelper.HttpPostFileAsync(PostUploadFileUrl,
                new[]
                {
                    @"C:\Users\liweihan.TUHU\Pictures\temp\icon.png",
                    @"C:\Users\liweihan.TUHU\Pictures\temp\7604648.jpg",
                }, formFields: new Dictionary<string, string>
                {
                    {"user", "abc" },
                    {"pwd","234" }
                });
            Console.WriteLine(response);
        }

        public static async Task HttpClientPostFile()
        {
            var client = new HttpClient();
            try
            {
                var response = await client.PostFileAsync(PostUploadFileUrl,
                    @"C:\Users\liweihan.TUHU\Pictures\temp\7604648.jpg");
                Console.WriteLine(await response.Content.ReadAsStringAsync());

                response = await client.PostFileAsync(PostUploadFileUrl, new[]
                {
                    @"C:\Users\liweihan.TUHU\Pictures\temp\icon.png",
                    @"C:\Users\liweihan.TUHU\Pictures\temp\7604648.jpg",
                });
                Console.WriteLine(await response.Content.ReadAsStringAsync());
            }
            finally
            {
                client.Dispose();
            }
        }

        public static void HttpRequesterTest()
        {
            //var client = new HttpRequester("https://weihanli.xyz");
            //var result = client.Execute();
            //Console.WriteLine(result);
            //client = new HttpRequest(PostUploadFileUrl, HttpMethod.Post);
            //client.WithFile(@"C:\Users\liweihan.TUHU\Pictures\temp\7604648.jpg");
            //result = client.Execute();
            //Console.WriteLine(result);

            //var response1 = new HttpRequester("https://initwords.com/")
            //    .ExecuteForResponse();
            //var sessionId = response1.Cookies["JSESSIONID"]?.Value ?? "E641209D81307143F8B2482B7B2C6ED2";

            //var client = new HttpRequester("https://initwords.com/login/authless/ajaxLogin.do", HttpMethod.Post);
            //client
            //    .WithHeaders(new Dictionary<string, string>
            //    {
            //        { "em-tokencode","a287f418-ed57-439c-bdb8-734baa00d9e4" },
            //        { "em-usercode","a01836e9-f566-46c5-b3df-528c65e78dbd" },
            //        {"cookie", $"Hm_lvt_49a5957871e8051bc1a873596375812d=1519034509; JSESSIONID=E641209D81307143F8B2482B7B2C6ED2; Hm_lpvt_49a5957871e8051bc1a873596375812d=1519034755" },
            //        { "origin", "https://initwords.com" },
            //        { "referer", "https://initwords.com/" },
            //        { "x-requested-with", "XMLHttpRequest" }
            //    })
            //    .WithParameters(new Dictionary<string, string>
            //    {
            //        { "loginType", "studentLogin" },
            //        { "siteName", "xfinit" },
            //        { "userId", "lby13460426337"},
            //        { "userPwd", "lby13460426337" }
            //    })
            //    .WithUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.167 Safari/537.36");
            //var response = client.Execute();
            //Console.WriteLine(response);

            var activateClient = new HttpRequester("https://initwords.com/book/ajaxRegisterBook.do", HttpMethod.Post);
            activateClient.WithHeaders(new Dictionary<string, string>
            {
                { "em-tokencode", "a287f418-ed57-439c-bdb8-734baa00d9e4" },
                { "em-usercode", "a01836e9-f566-46c5-b3df-528c65e78dbd" },
                { "cookie", $"Hm_lvt_49a5957871e8051bc1a873596375812d=1519034509; JSESSIONID=E641209D81307143F8B2482B7B2C6ED2; Hm_lpvt_49a5957871e8051bc1a873596375812d=1519034755" },
                { "origin", "https://initwords.com" },
                { "referer", "https://initwords.com/" },
                { "x-requested-with", "XMLHttpRequest" }
            })
            .WithFormParameters(new Dictionary<string, string>
            {
                { "moduleCode", "8a108cb74c7ae17a014c7d671d430771" },
                { "cardNo", "20180218018071452273218" },
                { "cardPwd", "bz6Bj568" },
                { "userCode", "a01836e9-f566-46c5-b3df-528c65e78dbd" }
            })
            .WithUserAgent("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/64.0.3282.167 Safari/537.36");
            var result = activateClient.Execute<TempResponseEntity>();
            Console.Write(result);
        }

        private class TempResponseEntity
        {
            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("code")]
            public int Code { get; set; }
        }
    }
}
