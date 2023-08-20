using WeihanLi.Common.Helpers;

namespace WeihanLi.Common;

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
    public static readonly GuidIdGenerator Instance = new();

    public string NewId() => Guid.NewGuid().ToString("N");
}

public sealed class SequentialGuidIdGenerator : IIdGenerator
{
    private readonly SequentialGuidType _sequentialGuidType;

    public SequentialGuidIdGenerator(SequentialGuidType sequentialGuidType)
    {
        _sequentialGuidType = sequentialGuidType;
    }

    public string NewId() => SequentialGuidGenerator.Create(_sequentialGuidType).ToString("N");
}
