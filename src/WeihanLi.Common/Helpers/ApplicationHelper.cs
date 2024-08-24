// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Reflection;
using System.Runtime.InteropServices;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public static class ApplicationHelper
{
    public static string ApplicationName =>
        Assembly.GetEntryAssembly()?.GetName().Name ?? AppDomain.CurrentDomain.FriendlyName;

    public static readonly string AppRoot = AppDomain.CurrentDomain.BaseDirectory;

    private static CancellationToken? _exitToken;
    public static CancellationToken ExitToken => _exitToken ??= InvokeHelper.GetExitTokenInternal();

    public static string MapPath(string virtualPath) => Path.Combine(AppRoot, virtualPath.TrimStart('~'));

    /// <summary>
    /// Get the library info from the assembly info
    /// </summary>
    /// <param name="type">type in the assembly</param>
    /// <returns>The assembly library info</returns>
    public static LibraryInfo GetLibraryInfo(Type type) => GetLibraryInfo(Guard.NotNull(type).Assembly);

    /// <summary>
    /// Get the library info from the assembly info
    /// </summary>
    /// <param name="assembly">assembly</param>
    /// <returns>The assembly library info</returns>
    public static LibraryInfo GetLibraryInfo(Assembly assembly)
    {
        Guard.NotNull(assembly);
        var assemblyInformation = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();
        var repositoryUrl = assembly.GetCustomAttributes<AssemblyMetadataAttribute>()
            .FirstOrDefault(x => nameof(LibraryInfo.RepositoryUrl).Equals(x.Key))?.Value ?? string.Empty;
        if (assemblyInformation is not null)
        {
            var informationalVersionSplit = assemblyInformation.InformationalVersion.Split('+');
            return new LibraryInfo()
            {
                LibraryVersion = informationalVersionSplit[0],
                LibraryHash = informationalVersionSplit.Length > 1 ? informationalVersionSplit[1] : string.Empty,
                RepositoryUrl = repositoryUrl
            };
        }
        return new LibraryInfo()
        {
            LibraryVersion = assembly.GetName().Version?.ToString() ?? string.Empty,
            LibraryHash = string.Empty,
            RepositoryUrl = repositoryUrl
        };
    }

    private static readonly Lazy<RuntimeInfo> _runtimeInfoLazy = new(GetRuntimeInfo);
    public static RuntimeInfo RuntimeInfo => _runtimeInfoLazy.Value;

    /// <summary>
    /// Get dotnet executable path
    /// </summary>
    public static string? GetDotnetPath()
    {
        return ResolvePath("dotnet");
    }

    public static string GetDotnetDirectory()
    {
        var environmentOverride = Environment.GetEnvironmentVariable("DOTNET_MSBUILD_SDK_RESOLVER_CLI_DIR");
        if (!string.IsNullOrEmpty(environmentOverride))
        {
            return environmentOverride;
        }
        environmentOverride = Environment.GetEnvironmentVariable("DOTNET_ROOT");
        if (!string.IsNullOrEmpty(environmentOverride))
        {
            return environmentOverride;
        }

        var dotnetExe = GetDotnetPath();

        if (dotnetExe.IsNotNullOrEmpty() && !InteropHelper.RunningOnWindows)
        {
            // e.g. on Linux the 'dotnet' command from PATH is a symbol link so we need to
            // resolve it to get the actual path to the binary
            dotnetExe = InteropHelper.Unix.RealPath(dotnetExe) ?? dotnetExe;
        }

        if (string.IsNullOrWhiteSpace(dotnetExe))
        {
#if NET6_0_OR_GREATER
            dotnetExe = Environment.ProcessPath;
#else
            dotnetExe = System.Diagnostics.Process.GetCurrentProcess().MainModule?.FileName;
#endif
        }

        return Guard.NotNull(Path.GetDirectoryName(dotnetExe));
    }

    public static string? ResolvePath(string execName) => ResolvePath(execName, ".exe");

    public static string? ResolvePath(string execName, string? windowsExt)
    {
        var executableName = execName;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            && !Path.HasExtension(execName)
            && !string.IsNullOrEmpty(windowsExt)
            )
        {
            executableName = $"{executableName}{windowsExt}";
        }
        var searchPaths = Guard.NotNull(Environment.GetEnvironmentVariable("PATH"))
            .Split(new[] { Path.PathSeparator }, options: StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim('"'))
            .ToArray();
        var commandPath = searchPaths
            .Where(p => !Path.GetInvalidPathChars().Any(p.Contains))
            .Select(p => Path.Combine(p, executableName))
            .FirstOrDefault(File.Exists);
        return commandPath;
    }

    private static RuntimeInfo GetRuntimeInfo()
    {
        var libInfo = GetLibraryInfo(typeof(object).Assembly);
#if NET6_0_OR_GREATER
#else
        var currentProcess = System.Diagnostics.Process.GetCurrentProcess();
#endif
        return new RuntimeInfo()
        {
            Version = Environment.Version.ToString(),
            ProcessorCount = Environment.ProcessorCount,
            FrameworkDescription = RuntimeInformation.FrameworkDescription,
            WorkingDirectory = Environment.CurrentDirectory,

#if NET6_0_OR_GREATER
            ProcessId = Environment.ProcessId,
            ProcessPath = Environment.ProcessPath ?? string.Empty,
            RuntimeIdentifier = RuntimeInformation.RuntimeIdentifier,
#else
            ProcessId = currentProcess.Id,
            ProcessPath = currentProcess.MainModule?.FileName ?? string.Empty,
#endif
            OSArchitecture = RuntimeInformation.OSArchitecture.ToString(),
            OSDescription = RuntimeInformation.OSDescription,
            OSVersion = Environment.OSVersion.ToString(),
            MachineName = Environment.MachineName,

            IsInContainer = IsInContainer(),
            IsInKubernetes = IsInKubernetesCluster(),

            LibraryVersion = libInfo.LibraryVersion,
            LibraryHash = libInfo.LibraryHash,
            RepositoryUrl = libInfo.RepositoryUrl,
        };
    }

    #region ContainerEnvironment
    private static bool IsInContainer()
    {
        // https://github.com/dotnet/dotnet-docker/blob/9b731e901dd4a343fc30da7b8b3ab7d305a4aff9/src/runtime-deps/7.0/cbl-mariner2.0/amd64/Dockerfile#L18
        return "true".Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
            StringComparison.OrdinalIgnoreCase);
    }

    private static readonly string ServiceAccountPath =
        Path.Combine(
        [
            $"{Path.DirectorySeparatorChar}var", "run", "secrets", "kubernetes.io", "serviceaccount",
        ]);
    private const string ServiceAccountTokenKeyFileName = "token";
    private const string ServiceAccountRootCAKeyFileName = "ca.crt";
    /// <summary>
    /// Whether running in k8s cluster
    /// </summary>
    /// <returns></returns>
    private static bool IsInKubernetesCluster()
    {
        var host = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_HOST");
        var port = Environment.GetEnvironmentVariable("KUBERNETES_SERVICE_PORT");

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port))
        {
            return false;
        }

        var tokenPath = Path.Combine(ServiceAccountPath, ServiceAccountTokenKeyFileName);
        if (!File.Exists(tokenPath))
        {
            return false;
        }
        var certPath = Path.Combine(ServiceAccountPath, ServiceAccountRootCAKeyFileName);
        return File.Exists(certPath);
    }
    #endregion ContainerEnvironment

}

public class LibraryInfo
{
    public required string LibraryVersion { get; init; }
    public required string LibraryHash { get; init; }
    public required string RepositoryUrl { get; init; }
}

public sealed class RuntimeInfo : LibraryInfo
{
    public required string Version { get; init; }
    public required string FrameworkDescription { get; init; }
    public required int ProcessorCount { get; init; }
    public required string OSArchitecture { get; init; }
    public required string OSDescription { get; init; }
    public required string OSVersion { get; init; }
    public required string MachineName { get; init; }

#if NET6_0_OR_GREATER
    public required string RuntimeIdentifier { get; init; }
#endif

    public required string WorkingDirectory { get; init; }
    public required int ProcessId { get; init; }
    public required string ProcessPath { get; init; }

    /// <summary>
    /// Is running in a container
    /// </summary>
    public required bool IsInContainer { get; init; }

    /// <summary>
    /// Is running in a Kubernetes cluster
    /// </summary>
    public required bool IsInKubernetes { get; init; }
}
