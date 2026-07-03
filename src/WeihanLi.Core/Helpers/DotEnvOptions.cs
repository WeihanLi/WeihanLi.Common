// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Helpers;

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
