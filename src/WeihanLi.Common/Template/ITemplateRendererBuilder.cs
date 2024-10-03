// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Template;

public interface ITemplateRendererBuilder
{
    ITemplateRendererBuilder UseTemplatePipe<TPipe>(TPipe pipe)
        where TPipe : class, ITemplatePipe;
    
    ITemplateRendererBuilder UseRenderMiddleware<TMiddleware>(TMiddleware middleware)
        where TMiddleware : class, IRenderMiddleware;
}

public interface ITemplateEngineBuilder : ITemplateRendererBuilder
{
    ITemplateRendererBuilder ConfigureOptions(Action<TemplateEngineOptions> optionsConfigure);
}
