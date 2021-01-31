namespace WeihanLi.Common.Models
{
    public class Category<TKey>
    {
        public TKey Id { get; set; } = default!;

        public string Name { get; set; } = null!;

        public TKey ParentId { get; set; } = default!;
    }

    public class Category : Category<int>
    {
    }

    public class CategoryWithDesc : Category<int>
    {
        public string? Description { get; set; }
    }
}
