using System;
using WeihanLi.Common.Helpers;

// ReSharper disable LocalizableElement

namespace DotNetFxSample
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("----------DotNetFxSample----------");

            // 数据库扩展
            // DataExtensionTest.MainTest();

            InvokeHelper.TryInvoke(LoggerTest.Test);
            var emptyArray = ArrayHelper.Empty<int>();
            Console.ReadLine();
        }
    }
}
