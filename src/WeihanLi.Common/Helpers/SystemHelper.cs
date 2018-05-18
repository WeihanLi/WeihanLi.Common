using System;

namespace WeihanLi.Common.Helpers
{
    public static class SystemHelper
    {
        public static readonly int ProcessorCount = Environment.ProcessorCount;

        public static readonly string MachineName = Environment.MachineName;

        public static readonly string OsType = Environment.OSVersion.VersionString;
    }
}
