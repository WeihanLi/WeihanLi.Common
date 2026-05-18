// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Models;

public class PagedRequest
{
    /// <summary>
    /// PageNumber
    /// 1 by default, 1 based
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
    /// PageSize
    /// 10 by default
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

public class OffsetRequest
{
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
