// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Templating;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddTemplating(this IServiceCollection services)
    {
        Guard.NotNull(services);

        services.TryAddSingleton<ITemplateParser, DefaultTemplateParser>();
        services.TryAddSingleton<ITemplateRenderer, DefaultTemplateRenderer>();
        services.TryAddSingleton<ITemplateEngine, TemplateEngine>();
        
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IRenderMiddleware, DefaultRenderMiddleware>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IRenderMiddleware, EnvRenderMiddleware>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IRenderMiddleware, ConfigurationRenderMiddleware>());
        services.TryAddSingleton(sp =>
        {
            var pipelineBuilder = PipelineBuilder.CreateAsync<TemplateRenderContext>();
            foreach (var middleware in sp.GetServices<IRenderMiddleware>())
            {
                pipelineBuilder.UseMiddleware(middleware);
            }
            return pipelineBuilder.Build();
        });
        
        return services;
    }
}
