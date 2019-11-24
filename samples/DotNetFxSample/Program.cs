using System;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging.Log4Net;

// ReSharper disable LocalizableElement

namespace DotNetFxSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LogHelper.AddLogProvider(new Log4NetLogHelperProvider());
            Console.WriteLine("----------DotNetFxSample----------");

            // 数据库扩展
            // DataExtensionTest.MainTest();

            InvokeHelper.TryInvoke(LoggerTest.Test);
            var emptyArray = ArrayHelper.Empty<int>();
            Console.ReadLine();
        }
    }
}
