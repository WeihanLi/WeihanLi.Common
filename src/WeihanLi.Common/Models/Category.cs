namespace WeihanLi.Common.Models
{
    public class Category<TKey>
    {
        public TKey Id { get; set; }

        public string Name { get; set; }

        public TKey ParentId { get; set; }
    }

    public class Category : Category<int>
    {
    }

    public class CategoryWithDesc : Category<int>
    {
        public string Description { get; set; }
    }
}
