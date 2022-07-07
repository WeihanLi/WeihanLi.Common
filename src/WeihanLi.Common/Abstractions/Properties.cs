// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Abstractions;

public interface IProperties
{
    IDictionary<string, object?> Properties { get; }
}
