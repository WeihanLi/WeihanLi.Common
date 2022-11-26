namespace WeihanLi.Common.Models;

public interface ISoftDeleteEntity
{ }

public interface ISoftDeleteEntityWithDeleted : ISoftDeleteEntity
{
    bool IsDeleted { get; set; }
}
