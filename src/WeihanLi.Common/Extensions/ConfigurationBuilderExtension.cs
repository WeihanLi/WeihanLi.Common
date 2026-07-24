// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using WeihanLi.Common;
using WeihanLi.Common.Helpers;

// ReSharper disable once CheckNamespace
namespace Microsoft.Extensions.Configuration;

/// <summary>
/// Provides extensions for <see cref="IConfigurationBuilder"/> instances.
/// </summary>
public static class ConfigurationBuilderExtension
{
    private const string DefaultDotEnvFileName = ".env";

    /// <summary>
    /// Adds <c>.env</c> file support to the specified <see cref="IConfigurationBuilder"/>.
    /// </summary>
    /// <param name="configurationBuilder">The configuration builder.</param>
    /// <param name="optionsSetup">The options setup delegate.</param>
    /// <param name="optional">Indicates whether the file is optional.</param>
    /// <param name="watching">Indicates whether changes should be watched and reloaded.</param>
    /// <returns>The same <see cref="IConfigurationBuilder"/> instance.</returns>
    public static IConfigurationBuilder AddDotEnv(this IConfigurationBuilder configurationBuilder, Action<DotEnvOptions>? optionsSetup = null, bool optional = true, bool watching = false)
    {
        Guard.NotNull(configurationBuilder);

        var options = CreateOptions(configurationBuilder, optionsSetup);
        var resolvedPath = ResolveDotEnvFilePath(options);
        var configuredPath = resolvedPath ?? Path.Combine(Guard.NotNullOrWhiteSpace(options.WorkingDirectory), DefaultDotEnvFileName);

        var source = new DotEnvConfigurationSource
        {
            Optional = optional,
            ReloadOnChange = watching,
            ResolvedPath = configuredPath,
            DotEnvOptions = CloneOptions(options, configuredPath)
        };

        return configurationBuilder.Add(source);
    }

    private static DotEnvOptions CreateOptions(IConfigurationBuilder configurationBuilder, Action<DotEnvOptions>? optionsSetup)
    {
        var options = new DotEnvOptions();
        optionsSetup?.Invoke(options);
        options.ValueConverter = Guard.NotNull(options.ValueConverter);

        if (!string.IsNullOrWhiteSpace(options.WorkingDirectory))
        {
            options.WorkingDirectory = Path.GetFullPath(options.WorkingDirectory);
            return options;
        }

        if (configurationBuilder.Properties.TryGetValue("BasePath", out var basePath)
            && basePath is string configuredBasePath
            && !string.IsNullOrWhiteSpace(configuredBasePath))
        {
            options.WorkingDirectory = configuredBasePath;
        }
        else if (configurationBuilder.Properties.TryGetValue("FileProvider", out var fileProvider)
                 && fileProvider?.GetType().GetProperty("Root")?.GetValue(fileProvider) is string providerRoot
                 && !string.IsNullOrWhiteSpace(providerRoot))
        {
            options.WorkingDirectory = providerRoot;
        }
        else
        {
            options.WorkingDirectory = AppContext.BaseDirectory;
        }

        return options;
    }

    private static DotEnvOptions CloneOptions(DotEnvOptions options, string resolvedPath) => new()
    {
        Recursive = false,
        ExportSupport = options.ExportSupport,
        TrimValues = options.TrimValues,
        WorkingDirectory = Path.GetDirectoryName(resolvedPath),
        ValueConverter = options.ValueConverter
    };

    private static string? ResolveDotEnvFilePath(DotEnvOptions options)
    {
        var workingDirectory = Path.GetFullPath(Guard.NotNullOrWhiteSpace(options.WorkingDirectory));
        if (!Directory.Exists(workingDirectory))
        {
            throw new DirectoryNotFoundException($"Working directory '{workingDirectory}' does not exist.");
        }

        var currentDirectory = workingDirectory;
        while (true)
        {
            var filePath = Path.Combine(currentDirectory, DefaultDotEnvFileName);
            if (File.Exists(filePath))
            {
                return filePath;
            }

            if (!options.Recursive)
            {
                return null;
            }

            var parent = Directory.GetParent(currentDirectory);
            if (parent is null)
            {
                return null;
            }

            currentDirectory = parent.FullName;
        }
    }

    private sealed class DotEnvConfigurationSource : IConfigurationSource
    {
        public required DotEnvOptions DotEnvOptions { get; init; }

        public required string ResolvedPath { get; init; }

        public bool Optional { get; init; }

        public bool ReloadOnChange { get; init; }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new DotEnvConfigurationProvider(this);
        }
    }

    private sealed class DotEnvConfigurationProvider : ConfigurationProvider, IDisposable
    {
        private readonly DotEnvConfigurationSource _source;
        private readonly object _reloadLock = new();
        private FileSystemWatcher? _watcher;
        private Timer? _reloadTimer;

        public DotEnvConfigurationProvider(DotEnvConfigurationSource source)
        {
            _source = source;
        }

        public override void Load()
        {
            Load(reload: false);

            if (_source.ReloadOnChange && _watcher is null)
            {
                InitializeWatcher();
            }
        }

        private void Load(bool reload)
        {
            if (!File.Exists(_source.ResolvedPath))
            {
                if (!reload && !_source.Optional)
                {
                    throw new FileNotFoundException($"The configuration file '{Path.GetFileName(_source.ResolvedPath)}' was not found and is not optional.", _source.ResolvedPath);
                }

                Data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
                return;
            }

            var values = EnvHelper.Read(options =>
            {
                options.Recursive = false;
                options.ExportSupport = _source.DotEnvOptions.ExportSupport;
                options.TrimValues = _source.DotEnvOptions.TrimValues;
                options.WorkingDirectory = _source.DotEnvOptions.WorkingDirectory;
                options.ValueConverter = _source.DotEnvOptions.ValueConverter;
            });

            var data = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            foreach (var pair in values)
            {
                data[NormalizeKey(pair.Key)] = pair.Value;
            }

            Data = data;
        }

        private void InitializeWatcher()
        {
            var directory = Guard.NotNull(Path.GetDirectoryName(_source.ResolvedPath));
            _watcher = new FileSystemWatcher(directory, Path.GetFileName(_source.ResolvedPath))
            {
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastWrite | NotifyFilters.Size
            };
            _watcher.Changed += OnFileChanged;
            _watcher.Created += OnFileChanged;
            _watcher.Deleted += OnFileChanged;
            _watcher.Renamed += OnFileRenamed;
            _watcher.EnableRaisingEvents = true;
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            ScheduleReload();
        }

        private void OnFileRenamed(object sender, RenamedEventArgs e)
        {
            ScheduleReload();
        }

        private void ScheduleReload()
        {
            lock (_reloadLock)
            {
                _reloadTimer?.Dispose();
                _reloadTimer = new Timer(_ =>
                {
                    lock (_reloadLock)
                    {
                        if (_watcher is null)
                        {
                            return;
                        }

                        try
                        {
                            Load(reload: true);
                            OnReload();
                        }
                        catch
                        {
                            // Ignore reload errors to avoid crashing the process on a background timer thread.
                        }
                    }
                }, null, TimeSpan.FromMilliseconds(250), Timeout.InfiniteTimeSpan);
            }
        }

        public void Dispose()
        {
            lock (_reloadLock)
            {
                _reloadTimer?.Dispose();
                _reloadTimer = null;
                if (_watcher is not null)
                {
                    _watcher.EnableRaisingEvents = false;
                    _watcher.Dispose();
                    _watcher = null;
                }
            }
        }

        private static string NormalizeKey(string key) => key.Replace("__", ConfigurationPath.KeyDelimiter);
    }
}
