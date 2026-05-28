// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

/// <summary>
/// Represents a page-based request.
/// </summary>
public class PagedRequest
{
    /// <summary>
    /// Gets or sets the one-based page number. The default value is 1.
    /// </summary>
    public int PageNum
    {
        get;
        set
        {
            if (value > 0)
            {
                field = value;
            }
        }
    } = 1;

    /// <summary>
    /// Gets or sets the page size. The default value is 10.
    /// </summary>
    public int PageSize
    {
        get;
        set
        {
            if (value > 0)
            {
                field = value;
            }
        }
    } = 10;
}

/// <summary>
/// Represents an offset-based request.
/// </summary>
public class OffsetRequest
{
    /// <summary>
    /// Gets or sets the zero-based offset.
    /// </summary>
    public int Offset
    {
        get => field;
        set
        {
            if (value >= 0)
            {
                field = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the requested item count. The default value is 10.
    /// </summary>
    public int Count
    {
        get => field;
        set
        {
            if (value > 0)
            {
                field = value;
            }
        }
    } = 10;
}
