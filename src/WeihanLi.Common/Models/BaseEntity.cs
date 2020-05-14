namespace WeihanLi.Common.Models
{
    public class BaseEntity<TKey>
    {
        public TKey Id { get; set; }
    }

    public class BaseEntityWithDeleted<TKey>
    {
        public TKey Id { get; set; }

        public bool IsDeleted { get; set; }
    }

    public class BaseEntity : BaseEntity<int>
    {
    }

    public class BaseEntityWithDeleted : BaseEntityWithDeleted<int>
    {
    }
}
