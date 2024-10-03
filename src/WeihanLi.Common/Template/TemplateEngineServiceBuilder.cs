// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.DependencyInjection;

namespace WeihanLi.Common.Template;

public interface ITemplateEngineServiceBuilder
{
    IServiceCollection Services { get; }
}

internal sealed class TemplateEngineServiceBuilder(IServiceCollection services) 
    : ITemplateEngineServiceBuilder
{
    public IServiceCollection Services { get; } = services;
}
