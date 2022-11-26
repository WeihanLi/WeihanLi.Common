namespace WeihanLi.Common.Models;

public interface ISoftDeleteEntity
{}

public interface ISoftDeleteEntityWithDeleted : ISoftDeleteEntity
{
    bool Deleted { get; set; }
}
