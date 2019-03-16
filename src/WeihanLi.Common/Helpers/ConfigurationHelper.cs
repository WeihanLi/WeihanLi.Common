using System;
using System.Collections.Specialized;
using System.Configuration;
using WeihanLi.Extensions;

#if NETSTANDARD2_0

using Microsoft.Extensions.Configuration;

#endif

namespace WeihanLi.Common.Helpers
{
    public static class ConfigurationHelper
    {
        /// <summary>
        /// ApplicationName
        /// </summary>
        [Obsolete("Please use ApplicationHelper.ApplicationName", true)]
        public static string ApplicationName => ApplicationHelper.ApplicationName;

        private static NameValueCollection _appSettings;

        static ConfigurationHelper()
        {
            _appSettings = ConfigurationManager.AppSettings;
        }

        /// <summary>
        /// 获取配置文件中AppSetting节点的相对路径对应的绝对路径
        /// </summary>
        /// <param name="key">key</param>
        /// <returns>绝对路径</returns>
        public static string AppSettingMapPath(string key) => ApplicationHelper.MapPath(AppSetting(key));

        /// <summary>
        /// 获取配置文件中AppSetting节点的值
        /// </summary>
        /// <param name="key">设置的键值</param>
        /// <returns>键值对应的值</returns>
        public static string AppSetting(string key) => _appSettings[key];

        /// <summary>
        /// 将虚拟路径转换为物理路径，相对路径转换为绝对路径
        /// </summary>
        /// <param name="virtualPath">虚拟路径</param>
        /// <returns>虚拟路径对应的物理路径</returns>
        [Obsolete("Please use ApplicationHelper.MapPath", true)]
        public static string MapPath(string virtualPath) => ApplicationHelper.MapPath(virtualPath);

        /// <summary>
        /// 获取配置文件中AppSetting节点的值
        /// </summary>
        /// <param name="key">设置的键值</param>
        /// <returns>键值对应的值</returns>
        public static T AppSetting<T>(string key) => _appSettings[key].StringToType<T>();

        public static T AppSetting<T>(string key, T defaultValue) => _appSettings[key].StringToType(defaultValue);

        public static T AppSetting<T>(string key, Func<T> defaultValueFactory) => _appSettings[key].StringToType(defaultValueFactory());

        public static bool AddAppSetting<T>(string key, T value) => AddAppSetting(key, value.ToJsonOrString());

        public static bool AddAppSetting(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Minimal);
            // Force a reload of a changed section.
            ConfigurationManager.RefreshSection("appSettings");
            _appSettings = ConfigurationManager.AppSettings;
            return true;
        }

        public static bool UpdateAppSetting<T>(string key, T value) => UpdateAppSetting(key, value.ToJsonOrString());

        public static bool UpdateAppSetting(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings.Remove(key);
            config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Minimal);
            // Force a reload of a changed section.
            ConfigurationManager.RefreshSection("appSettings");
            _appSettings = ConfigurationManager.AppSettings;
            return true;
        }

        /// <summary>
        /// 获取配置文件中ConnectionStrings节点的值
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>键值对应的连接字符串值</returns>
        public static string ConnectionString(string key) => ConfigurationManager.ConnectionStrings[key].ConnectionString;
    }
}
