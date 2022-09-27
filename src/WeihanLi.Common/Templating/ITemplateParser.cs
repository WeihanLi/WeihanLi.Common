// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Templating;
public interface ITemplateParser
{
    Task<TemplateRenderContext> ParseAsync(string text);
}
