namespace WeihanLi.Common.Models;

public record Category<TKey>
{
    public TKey Id { get; set; } = default!;

    public string Name { get; set; } = null!;

    public TKey ParentId { get; set; } = default!;
}

public record Category : Category<int>
{
}

public record CategoryWithDesc : Category<int>
{
    public string? Description { get; set; }
}
