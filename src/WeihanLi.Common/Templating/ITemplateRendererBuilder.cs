// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

namespace WeihanLi.Common.Templating;

public interface ITemplateRendererBuilder
{
    ITemplateRendererBuilder UseRenderMiddleware<TMiddleware>(TMiddleware middleware)
        where TMiddleware : class, IRenderMiddleware;
}

public interface ITemplateEngineBuilder : ITemplateRendererBuilder
{
    ITemplateRendererBuilder ConfigureOptions(Action<TemplateEngineOptions> configureOptionsAction);
}
