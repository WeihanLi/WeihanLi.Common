// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Reflection;
using System.Runtime;
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

    private static readonly Lazy<RuntimeInfo> LazyRuntimeInfo = new(GetRuntimeInfo);
    public static RuntimeInfo RuntimeInfo => LazyRuntimeInfo.Value;

    /// <summary>
    /// Get dotnet executable path
    /// </summary>
    public static string? GetDotnetPath()
    {
        var environmentOverride = Environment.GetEnvironmentVariable("DOTNET_ROOT");
        if (!string.IsNullOrEmpty(environmentOverride) && Directory.Exists(environmentOverride))
        {
            var execFileName =
#if NET6_0_OR_GREATER
                OperatingSystem.IsWindows()
#else
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
#endif
                ? "dotnet.exe"
                : "dotnet"
                ;
            var dotnetExePath = Path.Combine(environmentOverride, execFileName);
            if (File.Exists(dotnetExePath))
                return dotnetExePath;

            throw new InvalidOperationException($"dotnet executable file not found under specified DOTNET_ROOT {environmentOverride}");
        }
        return ResolvePath("dotnet");
    }

    public static string GetDotnetDirectory()
    {
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
        var runtimeInfo = new RuntimeInfo()
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
            UserName = Environment.UserName,

            IsServerGC = GCSettings.IsServerGC,

            IsInContainer = IsInContainer(),
            IsInKubernetes = IsInKubernetesCluster(),
            KubernetesNamespace = GetKubernetesNamespace(),

            LibraryVersion = libInfo.LibraryVersion,
            LibraryHash = libInfo.LibraryHash,
            RepositoryUrl = libInfo.RepositoryUrl,
        };
        return runtimeInfo;
    }

    #region ContainerEnvironment
    // container environment
    // https://github.com/dotnet/dotnet-docker/blob/d90d458deada9057d7889f76d58fc0a7194a0c06/src/runtime-deps/6.0/alpine3.20/amd64/Dockerfile#L7

    /// <summary>
    /// Whether running inside a container
    /// </summary>
    private static bool IsInContainer()
    {
        return "true".Equals(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"),
            StringComparison.OrdinalIgnoreCase);
    }

    // Kubernetes environment
    // https://github.com/kubernetes-client/csharp/blob/36a02046439d01f1256aed4e5071cb7f1b57d6eb/src/KubernetesClient/KubernetesClientConfiguration.InCluster.cs#L41
    private static readonly string ServiceAccountPath =
        Path.Combine(
        [
            $"{Path.DirectorySeparatorChar}var", "run", "secrets", "kubernetes.io", "serviceaccount",
        ]);
    private const string ServiceAccountTokenKeyFileName = "token";
    private const string ServiceAccountRootCAKeyFileName = "ca.crt";
    private const string ServiceAccountNamespaceFileName = "namespace";

    /// <summary>
    /// Whether running inside a k8s cluster
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

    /// <summary>
    /// Get Kubernetes namespace
    /// </summary>
    /// <returns>The namespace current workload in</returns>
    private static string? GetKubernetesNamespace()
    {
        var namespaceFilePath = Path.Combine(ServiceAccountPath, ServiceAccountNamespaceFileName);
        return File.Exists(namespaceFilePath) ? File.ReadAllText(namespaceFilePath).Trim() : null;
    }
    #endregion ContainerEnvironment
}

public class LibraryInfo
{
    public required string LibraryVersion { get; init; }
    public required string LibraryHash { get; init; }
    public required string RepositoryUrl { get; init; }
}

public class RuntimeInfo : LibraryInfo
{
    public required string Version { get; init; }
    public required string FrameworkDescription { get; init; }
    public required int ProcessorCount { get; init; }
    public required string OSArchitecture { get; init; }
    public required string OSDescription { get; init; }
    public required string OSVersion { get; init; }
    public required string MachineName { get; init; }
    public required string UserName { get; init; }

#if NET6_0_OR_GREATER
    public required string RuntimeIdentifier { get; init; }
#endif

    // GC
    /// <summary>Gets a value that indicates whether server garbage collection is enabled.</summary>
    /// <returns>
    /// <see langword="true" /> if server garbage collection is enabled; otherwise, <see langword="false" />.</returns>
    public required bool IsServerGC { get; init; }

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

    /// <summary>
    /// Kubernetes namespace when running in a Kubernetes cluster
    /// </summary>
    public string? KubernetesNamespace { get; init; }
}
