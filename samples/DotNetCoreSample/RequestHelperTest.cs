using System.Collections.Specialized;
using System.Diagnostics;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;

namespace DotNetCoreSample;

internal class RequestHelperTest
{
    private static readonly ILogHelperLogger Logger = LogHelper.GetLogger<RequestHelperTest>();

    public static void GetRequestParamTest()
    {
        var param = GetParamInfo("https://www.baidu.com/?tn=47018152_dg");
        Debug.Assert(param.HasKeys());
        Debug.Assert(param.Count == 1);

        param = GetParamInfo("http://tieba.baidu.com/f/index/forumpark?pcn=%E5%B0%8F%E8%AF%B4&pci=161&ct=0&rn=20&pn=1");
        Debug.Assert(param.HasKeys());
        Debug.Assert(param.Count == 5);

        param = GetParamInfo("https://www.baidu.com/?tn");
        Debug.Assert(param.HasKeys());

        param = GetParamInfo("https://www.baidu.com/?tn=");
        Debug.Assert(param["tn"] == string.Empty);
    }

    private static NameValueCollection GetParamInfo(string url)
    {
        var param = RequestHelper.GetParamCollection(url);
        Logger.Debug("\n url:{0} \n param info:\n {1}", url, string.Join(",", param.AllKeys.Select(p => p + ":" + param.Get(p))));
        return param;
    }
}
