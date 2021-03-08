namespace WeihanLi.Common.Models
{
    public class IdNameModel<TKey>
    {
        public TKey Id { get; set; } = default!;
        public string Name { get; set; } = null!;
    }

    public class IdNameDescModel<TKey> : IdNameModel<TKey>
    {
        public string? Description { get; set; }
    }

    public class IdNameModel : IdNameModel<int> { }

    public class IdNameDescModel : IdNameDescModel<int> { }
}
