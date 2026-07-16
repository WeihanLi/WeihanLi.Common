// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.Text;
using WeihanLi.Common;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public static class EnvHelper
{
    private const string DefaultDotEnvFileName = ".env";

    [return: NotNullIfNotNull(nameof(defaultValue))]
    public static string? Val(string envName, string? defaultValue = null)
    {
        return Environment.GetEnvironmentVariable(envName) ?? defaultValue;
    }

    public static string RequiredVal(string envName)
    {
        envName = WeihanLi.Common.Guard.NotNullOrWhiteSpace(envName);
        return Environment.GetEnvironmentVariable(envName) ?? throw new InvalidOperationException($"Environment variable `{envName}` not found.");
    }

    public static bool BooleanVal(string envName, bool defaultValue = false)
    {
        var val = Environment.GetEnvironmentVariable(envName);
        return val.ToBoolean(defaultValue);
    }

    /// <summary>
    /// Loads values from a <c>.env</c> file into the current process environment variables.
    /// </summary>
    /// <param name="optionsSetup">The options setup delegate.</param>
    public static void Load(Action<DotEnvOptions>? optionsSetup = null)
    {
        foreach (var pair in Read(optionsSetup))
        {
            Environment.SetEnvironmentVariable(pair.Key, pair.Value);
        }
    }

    /// <summary>
    /// Reads values from a <c>.env</c> file without mutating the current process environment variables.
    /// </summary>
    /// <param name="optionsSetup">The options setup delegate.</param>
    /// <returns>The parsed key/value pairs.</returns>
    public static Dictionary<string, string> Read(Action<DotEnvOptions>? optionsSetup = null)
    {
        var options = CreateOptions(optionsSetup);
        var filePath = ResolveDotEnvFilePath(options);
        if (filePath is null)
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return Parse(stream, options, filePath);
    }

    /// <summary>
    /// Loads values from a <c>.env</c> file into the current process environment variables asynchronously.
    /// </summary>
    /// <param name="optionsSetup">The options setup delegate.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public static async ValueTask LoadAsync(Action<DotEnvOptions>? optionsSetup = null)
    {
        foreach (var pair in await ReadAsync(optionsSetup).ConfigureAwait(false))
        {
            Environment.SetEnvironmentVariable(pair.Key, pair.Value);
        }
    }

    /// <summary>
    /// Reads values from a <c>.env</c> file asynchronously without mutating the current process environment variables.
    /// </summary>
    /// <param name="optionsSetup">The options setup delegate.</param>
    /// <returns>A task that contains the parsed key/value pairs.</returns>
    public static async ValueTask<Dictionary<string, string>> ReadAsync(Action<DotEnvOptions>? optionsSetup = null)
    {
        var options = CreateOptions(optionsSetup);
        var filePath = ResolveDotEnvFilePath(options);
        if (filePath is null)
        {
            return new Dictionary<string, string>(StringComparer.Ordinal);
        }

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, true);
        return await ParseAsync(stream, options, filePath).ConfigureAwait(false);
    }

    private static DotEnvOptions CreateOptions(Action<DotEnvOptions>? optionsSetup)
    {
        var options = new DotEnvOptions
        {
            WorkingDirectory = Environment.CurrentDirectory
        };
        optionsSetup?.Invoke(options);
        options.ValueConverter = Guard.NotNull(options.ValueConverter);
        options.WorkingDirectory = string.IsNullOrWhiteSpace(options.WorkingDirectory)
            ? Environment.CurrentDirectory
            : options.WorkingDirectory;
        return options;
    }

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

    private static Dictionary<string, string> Parse(Stream stream, DotEnvOptions options, string filePath)
    {
        using var reader = CreateReader(stream);
        return ParseCore(reader.ReadLine, options, filePath);
    }

    private static async ValueTask<Dictionary<string, string>> ParseAsync(Stream stream, DotEnvOptions options, string filePath)
    {
        using var reader = CreateReader(stream);
        return await ParseCoreAsync(reader.ReadLineAsync, options, filePath).ConfigureAwait(false);
    }

    private static StreamReader CreateReader(Stream stream) => new(stream, Encoding.UTF8, true, 4096, false);

    private static Dictionary<string, string> ParseCore(Func<string?> lineReader, DotEnvOptions options, string filePath)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        var lineNumber = 0;
        string? line;
        while ((line = lineReader()) is not null)
        {
            lineNumber++;
            if (!TryParseLine(line, lineNumber, options, filePath, out var pair))
            {
                continue;
            }

            values[pair.Key] = pair.Value;
        }

        return values;
    }

    private static async ValueTask<Dictionary<string, string>> ParseCoreAsync(Func<Task<string?>> lineReader, DotEnvOptions options, string filePath)
    {
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        var lineNumber = 0;
        string? line;
        while ((line = await lineReader().ConfigureAwait(false)) is not null)
        {
            lineNumber++;
            if (!TryParseLine(line, lineNumber, options, filePath, out var pair))
            {
                continue;
            }

            values[pair.Key] = pair.Value;
        }

        return values;
    }

    private static bool TryParseLine(string line, int lineNumber, DotEnvOptions options, string filePath, out KeyValuePair<string, string> pair)
    {
        pair = default;
        if (string.IsNullOrWhiteSpace(line))
        {
            return false;
        }

        var trimmedStart = line.TrimStart();
        if (trimmedStart.StartsWith("#", StringComparison.Ordinal))
        {
            return false;
        }

        if (!options.ExportSupport && trimmedStart.StartsWith("export ", StringComparison.Ordinal))
        {
            throw CreateFormatException(filePath, lineNumber, line);
        }

        var entry = options.ExportSupport && trimmedStart.StartsWith("export ", StringComparison.Ordinal)
            ? trimmedStart["export ".Length..]
            : line;

        var separatorIndex = entry.IndexOf('=');
        if (separatorIndex < 0)
        {
            throw CreateFormatException(filePath, lineNumber, line);
        }

        var key = entry[..separatorIndex].Trim();
        if (key.Length == 0)
        {
            throw CreateFormatException(filePath, lineNumber, line);
        }

        var value = ParseValue(entry[(separatorIndex + 1)..], options);
        value = Guard.NotNull(options.ValueConverter(value));
        pair = new KeyValuePair<string, string>(key, value);
        return true;
    }

    private static string ParseValue(string value, DotEnvOptions options)
    {
        var trimmedValue = value.Trim();
        if (trimmedValue.Length >= 2)
        {
            var quote = trimmedValue[0];
            if ((quote == '"' || quote == '\'') && trimmedValue[^1] == quote)
            {
                return trimmedValue[1..^1];
            }
        }

        return options.TrimValues ? trimmedValue : value;
    }

    private static FormatException CreateFormatException(string filePath, int lineNumber, string line) =>
        new($"Invalid .env entry at line {lineNumber} in '{filePath}'");
}

/// <summary>
/// Represents options for loading and parsing <c>.env</c> files.
/// </summary>
public sealed class DotEnvOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether parent directories should be searched for a <c>.env</c> file.
    /// </summary>
    public bool Recursive { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the <c>export </c> prefix should be supported.
    /// </summary>
    public bool ExportSupport { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether parsed values should be trimmed.
    /// </summary>
    public bool TrimValues { get; set; } = true;

    /// <summary>
    /// Gets or sets the working directory used to locate the <c>.env</c> file.
    /// </summary>
    public string? WorkingDirectory { get; set; }

    /// <summary>
    /// Gets or sets the converter used to transform parsed values before they are returned or loaded.
    /// </summary>
    public Func<string, string> ValueConverter { get; set; } = static value => value;
}
