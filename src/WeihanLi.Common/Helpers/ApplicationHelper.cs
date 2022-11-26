using System.Reflection;
using System.Runtime.InteropServices;

namespace WeihanLi.Common.Helpers;

public static class ApplicationHelper
{
    public static string ApplicationName =>
        Assembly.GetEntryAssembly()?.GetName().Name ?? AppDomain.CurrentDomain.FriendlyName;

    public static readonly string AppRoot = AppDomain.CurrentDomain.BaseDirectory;

    public static string MapPath(string virtualPath) => AppRoot + virtualPath.TrimStart('~');

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
            if (Version.TryParse(informationalVersionSplit[0], out var version))
            {
                return new LibraryInfo()
                {
                    LibraryVersion = version,
                    LibraryHash = informationalVersionSplit.Length > 1 ? informationalVersionSplit[1] : string.Empty,
                    RepositoryUrl = repositoryUrl
                };
            }
        }
        return new LibraryInfo()
        {
            LibraryVersion = assembly.GetName().Version,
            LibraryHash = string.Empty,
            RepositoryUrl = repositoryUrl
        };
    }

    private static readonly Lazy<RuntimeInfo> _runtimeInfoLazy = new(GetRuntimeInfo);
    public static RuntimeInfo RuntimeInfo => _runtimeInfoLazy.Value;

    /// <summary>
    /// Get dotnet executable path
    /// </summary>
    public static string GetDotnetPath()
    {
        var executableName =
            $"dotnet{(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? ".exe" : string.Empty)}";
        var searchPaths = Guard.NotNull(Environment.GetEnvironmentVariable("PATH"))
            .Split(new[] { Path.PathSeparator }, options: StringSplitOptions.RemoveEmptyEntries)
            .Select(p => p.Trim('"'))
            .ToArray();
        var commandPath = searchPaths
            .Where(p => !Path.GetInvalidPathChars().Any(p.Contains))
            .Select(p => Path.Combine(p, executableName))
            .First(File.Exists);
        return commandPath;
    }

    private static RuntimeInfo GetRuntimeInfo()
    {
        var libInfo = GetLibraryInfo(typeof(object).Assembly);
        return new RuntimeInfo()
        {
            Version = Environment.Version,
            ProcessorCount = Environment.ProcessorCount,
            FrameworkDescription = RuntimeInformation.FrameworkDescription,
            WorkingDirectory = Environment.CurrentDirectory,

#if NET6_0_OR_GREATER
            RuntimeIdentifier = RuntimeInformation.RuntimeIdentifier,
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
        Path.Combine(new string[]
        {
            $"{Path.DirectorySeparatorChar}var", "run", "secrets", "kubernetes.io", "serviceaccount",
        });
    private const string ServiceAccountTokenKeyFileName = "token";
    private const string ServiceAccountRootCAKeyFileName = "ca.crt";
    /// <summary>
    /// Whether running in 
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
    public required Version LibraryVersion { get; init; }
    public required string LibraryHash { get; init; }
    public required string RepositoryUrl { get; init; }
}

public sealed class RuntimeInfo : LibraryInfo
{
    public required Version Version { get; init; }
    public required string FrameworkDescription { get; init; }

    public required int ProcessorCount { get; init; }
    public required string OSArchitecture { get; init; }
    public required string OSDescription { get; init; }
    public required string OSVersion { get; init; }
    public required string MachineName { get; init; }

    public required string WorkingDirectory { get; init; }

    /// <summary>
    /// Is running in a container
    /// </summary>
    public required bool IsInContainer { get; init; }

    /// <summary>
    /// Is running in a kubernetes cluster
    /// </summary>
    public required bool IsInKubernetes { get; init; }

#if NET6_0_OR_GREATER
    public required string RuntimeIdentifier { get; init; }
#endif
}
