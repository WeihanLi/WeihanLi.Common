// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Template;

public interface ITemplatePipe
{
    string Name { get; }
    object? Convert(object? value, params ReadOnlySpan<string> args);
}
