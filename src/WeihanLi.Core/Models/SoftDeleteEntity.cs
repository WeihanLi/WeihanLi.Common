namespace WeihanLi.Common.Models;

/// <summary>
/// Marker interface for soft-delete entities.
/// </summary>
public interface ISoftDeleteEntity;

/// <summary>
/// Represents a soft-delete entity with a deletion flag.
/// </summary>
public interface ISoftDeleteEntityWithDeleted : ISoftDeleteEntity
{
    /// <summary>
    /// Gets or sets a value indicating whether the entity is deleted.
    /// </summary>
    bool IsDeleted { get; set; }
}
