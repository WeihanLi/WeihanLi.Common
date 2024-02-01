using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Services;

/// <summary>
/// IdGenerator
/// </summary>
public interface IIdGenerator
{
    /// <summary>
    /// Generate a new id
    /// </summary>
    /// <returns>new id</returns>
    string NewId();
}

/// <summary>
/// IdGenerator based on Guid
/// </summary>
public sealed class GuidIdGenerator : IIdGenerator
{
    public static GuidIdGenerator Instance { get; } = new();

    public string NewId() => Guid.NewGuid().ToString("N");
}

public sealed class SequentialGuidIdGenerator(SequentialGuidType sequentialGuidType) : IIdGenerator
{
    private readonly SequentialGuidType _sequentialGuidType = sequentialGuidType;

    public string NewId() => SequentialGuidGenerator.Create(_sequentialGuidType).ToString("N");
}
