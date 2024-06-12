// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using System.ComponentModel.DataAnnotations;

namespace WeihanLi.Common.Models;

public record Category<TKey>
{
    public TKey Id { get; set; } = default!;

    [StringLength(256)]
    [Required]
    public string Name { get; set; } = null!;

    public TKey ParentId { get; set; } = default!;
}

public record Category : Category<int>;

public record CategoryWithDesc : Category<int>
{
    [StringLength(2048)]
    public string? Description { get; set; }
}
