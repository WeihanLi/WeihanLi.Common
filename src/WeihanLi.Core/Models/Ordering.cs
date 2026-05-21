// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

public enum Ordering
{
    Ascending = 0,
    Descending = 1
}

[Flags]
public enum RangeInclusion
{
    None = 0,
    IncludeLowerBound = 1,
    IncludeUpperBound = 2
}
