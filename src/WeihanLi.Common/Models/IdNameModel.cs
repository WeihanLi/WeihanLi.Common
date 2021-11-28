namespace WeihanLi.Common.Models
{
    public record IdNameModel<TKey>
    {
        public TKey Id { get; set; } = default!;
        public string Name { get; set; } = null!;

        public void Deconstruct(out TKey id, out string name)
        {
            id = Id;
            name = Name;
        }
    }

    public record IdNameDescModel<TKey> : IdNameModel<TKey>
    {
        public string? Description { get; set; }

        public void Deconstruct(out TKey id, out string name, out string? description)
        {
            id = Id;
            name = Name;
            description = Description;
        }
    }

    public record IdNameModel : IdNameModel<int> { }

    public record IdNameDescModel : IdNameDescModel<int> { }
}
