using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Services;

/// <summary>
/// Generates string identifiers.
/// </summary>
public interface IIdGenerator
{
    /// <summary>
    /// Generates a new identifier.
    /// </summary>
    /// <returns>The generated identifier.</returns>
    string NewId();
}

/// <summary>
/// Identifier generator based on <see cref="Guid.NewGuid"/>.
/// </summary>
public sealed class GuidIdGenerator : IIdGenerator
{
    /// <summary>
    /// Gets the singleton instance.
    /// </summary>
    public static GuidIdGenerator Instance { get; } = new();

    /// <summary>
    /// Generates a new compact GUID string without separators.
    /// </summary>
    /// <returns>The generated identifier.</returns>
    public string NewId() => Guid.NewGuid().ToString("N");
}

/// <summary>
/// Identifier generator based on sequential GUID values.
/// </summary>
/// <param name="sequentialGuidType">The sequential GUID layout to use.</param>
public sealed class SequentialGuidIdGenerator(SequentialGuidType sequentialGuidType) : IIdGenerator
{
    private readonly SequentialGuidType _sequentialGuidType = sequentialGuidType;

    /// <summary>
    /// Generates a new compact sequential GUID string without separators.
    /// </summary>
    /// <returns>The generated identifier.</returns>
    public string NewId() => SequentialGuidGenerator.Create(_sequentialGuidType).ToString("N");
}
