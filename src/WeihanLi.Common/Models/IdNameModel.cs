namespace WeihanLi.Common.Models
{
    public class IdNameModel<TKey>
    {
        public TKey Id { get; set; } = default!;
        public string Name { get; set; } = null!;

        public void Deconstruct(out TKey id, out string name)
        {
            id = Id;
            name = Name;
        }
    }

    public class IdNameDescModel<TKey> : IdNameModel<TKey>
    {
        public string? Description { get; set; }

        public void Deconstruct(out TKey id, out string name, out string? description)
        {
            id = Id;
            name = Name;
            description = Description;
        }
    }

    public class IdNameModel : IdNameModel<int> { }

    public class IdNameDescModel : IdNameDescModel<int> { }
}
