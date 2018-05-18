using System.Collections.Specialized;
using System.Web;

namespace WeihanLi.Common.Helpers
{
    public static class RequestHelper
    {
        /// <summary>
        ///     获得参数列表
        /// </summary>
        /// <param name="url">数据</param>
        /// <returns></returns>
        public static NameValueCollection GetParamCollection(string url)
        {
            return HttpUtility.ParseQueryString(url);
        }
    }
}
