using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using JetBrains.Annotations;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Extensions
{
    public static class RequestExtension
    {
#if NET45

        #region 客户端信息

        //浏览器列表
        private static readonly string[] BrowserNames =
            {"ie", "chrome", "edge", "mozilla", "netscape", "firefox", "opera", "konqueror"};

        //搜索引擎列表
        private static readonly string[] SearchEngines =
            {"baidu", "google", "360", "sogou", "bing", "msn", "sohu", "soso", "sina", "163", "yahoo", "jikeu"};

        //meta正则表达式
        private static readonly Regex metaRegex = new Regex("<meta([^<]*)charset=([^<]*)[\"']",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        //侦测移动浏览器正则表达式
        private static Regex _detectMobileBrowserRegexB = new Regex(
            @"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        private static Regex _detectMobileBrowserRegexV = new Regex(
            @"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-",
            RegexOptions.IgnoreCase | RegexOptions.Multiline);

        /// <summary>
        /// 是否是Ajax请求
        /// </summary>
        /// <returns></returns>
        public static bool IsAjax([NotNull]this HttpRequestBase request)
            => "XMLHttpRequest".Equals(request.Headers["X-Requested-With"], StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// 获得查询字符串中的值
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string GetQueryString([NotNull]this HttpRequestBase request, string key, string defaultValue) => request.QueryString[key] ?? defaultValue;

        /// <summary>
        /// 获得查询字符串中的值
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string GetQueryString([NotNull]this HttpRequestBase request, string key) => request.GetQueryString(key, "");

        /// <summary>
        /// 获得表单中的值
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string GetFormString([NotNull]this HttpRequestBase request, string key, string defaultValue)
            => request.Form[key] ?? defaultValue;

        /// <summary>
        /// 获得表单中的值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string GetFormString([NotNull]this HttpRequestBase request, string key) => request.GetFormString(key, "");

        /// <summary>
        /// 获得请求中的值
        /// </summary>
        /// <param name="request"></param>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string GetRequestString([NotNull]this HttpRequestBase request, string key, string defaultValue) => request[key] ?? defaultValue;

        /// <summary>
        /// 获得请求中的值
        /// </summary>
        /// <param name="request">request</param>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string GetRequestString([NotNull]this HttpRequestBase request, string key) => request.GetRequestString(key, "");

        /// <summary>
        /// 获得上次请求的url
        /// </summary>
        /// <returns></returns>
        public static string GetUrlReferrer([NotNull]this HttpRequestBase request) => request.UrlReferrer?.ToString() ?? string.Empty;

        /// <summary>
        /// 获得请求的主机部分
        /// </summary>
        /// <returns></returns>
        public static string GetHost([NotNull]this HttpRequestBase request) => request.Url.Host;

        /// <summary>
        /// 获得请求的url
        /// </summary>
        /// <returns></returns>
        public static string GetUrl([NotNull]this HttpRequestBase request) => request.Url.ToString();

        /// <summary>
        /// 获得请求的ip
        /// </summary>
        /// <returns></returns>
        public static string GetIP([NotNull]this HttpRequestBase request)
        {
            string ip = string.Empty;
            if (request.ServerVariables["HTTP_VIA"] != null)
            {
                ip = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            }
            else
            {
                ip = request.ServerVariables["REMOTE_ADDR"];
            }
            if (string.IsNullOrEmpty(ip) || !ValidateHelper.IsIP(ip))
                ip = "127.0.0.1";
            return ip;
        }

        /// <summary>
        /// 获得请求的浏览器类型
        /// </summary>
        /// <returns></returns>
        public static string GetBrowserType()
        {
            string type = HttpContext.Current.Request.Browser.Type;
            if (string.IsNullOrEmpty(type) || type == "unknown")
                return "未知";

            return type.ToLower();
        }

        /// <summary>
        /// 获得请求的浏览器名称
        /// </summary>
        /// <returns></returns>
        public static string GetBrowserName()
        {
            // WARNING:这个方法的实现有BUG，Edge 会变成 Chrome
            string name = HttpContext.Current.Request.Browser.Browser.ToLower();
            if (string.IsNullOrEmpty(name) || name == "unknown")
                return "未知";
            return name.ToLower();
        }

        /// <summary>
        /// 获得请求的浏览器版本
        /// </summary>
        /// <returns></returns>
        public static string GetBrowserVersion()
        {
            string version = HttpContext.Current.Request.Browser.Version;
            if (string.IsNullOrEmpty(version) || version == "unknown")
                return "未知";

            return version;
        }

        /// <summary>
        /// 获得请求客户端的操作系统类型
        /// </summary>
        /// <returns></returns>
        public static string GetOsType([NotNull]this HttpRequestBase request)
        {
            string userAgent = request.UserAgent;
            if (userAgent == null)
                return "Unknown";

            string type = null;
            if (userAgent.Contains("NT 6.1"))
                type = "Windows 7";
            else if (userAgent.Contains("NT 5.1"))
                type = "Windows XP";
            else if (userAgent.Contains("NT 6.2"))
                type = "Windows 8";
            else if (userAgent.Contains("NT 10.0"))
                type = "Windows 10";
            else if (userAgent.Contains("android"))
                type = "Android";
            else if (userAgent.Contains("iphone"))
                type = "IPhone";
            else if (userAgent.Contains("Mac"))
                type = "Mac";
            else if (userAgent.Contains("NT 6.0"))
                type = "Windows Vista";
            else if (userAgent.Contains("NT 5.2"))
                type = "Windows 2003";
            else if (userAgent.Contains("NT 5.0"))
                type = "Windows 2000";
            else if (userAgent.Contains("98"))
                type = "Windows 98";
            else if (userAgent.Contains("95"))
                type = "Windows 95";
            else if (userAgent.Contains("Me"))
                type = "Windows Me";
            else if (userAgent.Contains("NT 4"))
                type = "Windows NT4";
            else if (userAgent.Contains("Unix"))
                type = "UNIX";
            else if (userAgent.Contains("Linux"))
                type = "Linux";
            else if (userAgent.Contains("SunOS"))
                type = "SunOS";
            else
                type = "未知";

            return type;
        }

        /// <summary>
        /// 获得请求客户端的操作系统名称
        /// </summary>
        /// <returns></returns>
        public static string GetOsName([NotNull]this HttpRequestBase request) => request.Browser.Platform ?? "Unknown";

        /// <summary>
        /// 判断是否是浏览器请求
        /// </summary>
        /// <returns></returns>
        public static bool IsBrowser()
        {
            string name = GetBrowserName();
            foreach (string item in BrowserNames)
            {
                if (name.Contains(item))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 是否是移动设备请求
        /// </summary>
        /// <returns></returns>
        public static bool IsMobile()
        {
            var userAgent = HttpContext.Current.Request.UserAgent;
            if (userAgent == null)
                return false;

            if (_detectMobileBrowserRegexB.IsMatch(userAgent) || _detectMobileBrowserRegexV.IsMatch(userAgent.Substring(0, 4)))
                return true;

            return false;
        }

        /// <summary>
        /// 判断是否是搜索引擎爬虫请求
        /// </summary>
        /// <returns></returns>
        public static bool IsCrawler([NotNull]this HttpRequestBase request)
        {
            bool result = request.Browser.Crawler;
            if (!result)
            {
                var referrer = request.GetUrlReferrer();
                if (referrer.IsNotNullOrWhiteSpace() &&
                    SearchEngines.Any(item => referrer.Contains(item)))
                    return true;
            }
            return result;
        }

        #endregion 客户端信息

#endif
    }
}
