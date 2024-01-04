// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Template;

public interface ITemplateEngine
{
    Task<string> RenderAsync(string text, object? globals = null);
}
