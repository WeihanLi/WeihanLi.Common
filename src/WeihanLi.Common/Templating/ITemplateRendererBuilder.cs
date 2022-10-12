// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Templating;
public interface ITemplateRendererBuilder
{
    ITemplateRendererBuilder Use(Func<TemplateRenderContext, Func<TemplateRenderContext, Task>, Task> middleware);
    ITemplateRendererBuilder ConfigureOptions(Action<TemplateOptions> configureOptionsAction);
    ITemplateRenderer Build();
}
