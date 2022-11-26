// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Runtime.InteropServices;

namespace WeihanLi.Common.Helpers;

// https://github.dev/dotnet/sdk/blob/5c99629b15ef721440e61007b88088d0cc1d3c49/src/Resolvers/Microsoft.DotNet.NativeWrapper/Interop.cs#L182
internal static class Interop
{
    public static readonly bool RunningOnWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

    public static class Unix
    {
        // Ansi marshaling on Unix is actually UTF8
        // ReSharper disable InconsistentNaming
        private const CharSet UTF8 = CharSet.Ansi;
        private static string? PtrToStringUTF8(IntPtr ptr) => Marshal.PtrToStringAnsi(ptr);

        [DllImport("libc", CharSet = UTF8, ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr realpath(string path, IntPtr buffer);

        [DllImport("libc", ExactSpelling = true, CallingConvention = CallingConvention.Cdecl)]
        private static extern void free(IntPtr ptr);

        public static string? RealPath(string path)
        {
            var ptr = realpath(path, IntPtr.Zero);
            var result = PtrToStringUTF8(ptr);
            free(ptr);
            return result;
        }
    }
}
