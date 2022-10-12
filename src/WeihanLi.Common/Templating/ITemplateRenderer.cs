// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Templating;
public interface ITemplateRenderer
{
    Task<string> RenderAsync(TemplateRenderContext template, object globals);
}
