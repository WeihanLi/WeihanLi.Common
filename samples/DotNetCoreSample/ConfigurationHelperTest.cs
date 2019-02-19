using System;
using System.Reflection;
using WeihanLi.Common.Helpers;
using WeihanLi.Common.Logging;

namespace DotNetCoreSample
{
    internal class ConfigurationHelperTest
    {
        private static readonly ILogHelperLogger Logger = LogHelper.GetLogger<ConfigurationHelperTest>();

        public static void TestConfigurationHelper()
        {
            Console.WriteLine($"Assembly.GetEntryAssembly().GetName().Name :{Assembly.GetEntryAssembly().GetName().Name} \nAppDomain.CurrentDomain.FriendlyName :{AppDomain.CurrentDomain.FriendlyName}");
            Console.WriteLine("ApplicationName:{0}", ApplicationHelper.ApplicationName);
            var key1 = ConfigurationHelper.AppSetting("key1");
            var key2 = ConfigurationHelper.AppSetting<int>("key2");
            var key3 = ConfigurationHelper.AppSetting<bool>("key3");
            var path = ConfigurationHelper.AppSettingMapPath("log4net");
            Console.WriteLine($"key1:{key1},key2:{key2},key3:{key3},path:{path}");

            ConfigurationHelper.UpdateAppSetting("key1", "key111");
            Console.WriteLine(ConfigurationHelper.AppSetting("key1"));
            ConfigurationHelper.AddAppSetting("jsonKey", new TestEntity
            {
                PKID = 1,
                Token = Guid.NewGuid().ToString("N"),
                CreatedTime = DateTime.Now
            });
            Console.WriteLine(ConfigurationHelper.AppSetting("jsonKey"));
        }
    }
}
