using System.Collections.Specialized;
using System.Web;

namespace WeihanLi.Common.Helpers;

public static class RequestHelper
{
    /// <summary>
    /// Get QueryString
    /// </summary>
    /// <param name="url">url</param>
    /// <returns>QueryString</returns>
    public static NameValueCollection GetParamCollection(string url)
    {
        return HttpUtility.ParseQueryString(url);
    }
}
