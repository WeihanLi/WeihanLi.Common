using System;
using System.Reflection;

namespace WeihanLi.Common.Helpers
{
    public static class ApplicationHelper
    {
        /// <summary>
        /// ApplicationName
        /// </summary>
        public static string ApplicationName =>
#if NET45
            System.Web.Hosting.HostingEnvironment.IsHosted ? System.Web.Hosting.HostingEnvironment.SiteName : Assembly.GetEntryAssembly()?.GetName().Name ?? AppDomain.CurrentDomain.FriendlyName;
#else
            Assembly.GetEntryAssembly()?.GetName().Name ?? AppDomain.CurrentDomain.FriendlyName;

#endif

        /// <summary>
        /// 应用根目录
        /// </summary>
#if NET45
        internal static readonly string AppRoot = System.Web.Hosting.HostingEnvironment.IsHosted ? System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath : AppDomain.CurrentDomain.BaseDirectory;
#else
        internal static readonly string AppRoot = AppDomain.CurrentDomain.BaseDirectory;
#endif

        /// <summary>
        /// 将虚拟路径转换为物理路径，相对路径转换为绝对路径
        /// </summary>
        /// <param name="virtualPath">虚拟路径</param>
        /// <returns>虚拟路径对应的物理路径</returns>
        public static string MapPath(string virtualPath) => AppRoot + virtualPath.TrimStart('~');
    }
}
