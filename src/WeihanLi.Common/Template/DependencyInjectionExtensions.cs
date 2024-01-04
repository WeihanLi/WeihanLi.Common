// Copyright (c) Weihan Li. All rights reserved.
// Licensed under the Apache license.

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Template;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddTemplating(this IServiceCollection services, Action<TemplateEngineOptions>? optionsConfigure = null)
    {
        Guard.NotNull(services);
        if (services.Any(x => x.ServiceType == typeof(ITemplateEngine)))
            throw new InvalidOperationException("Templating services had been registered");

        if (optionsConfigure != null)
            services.AddOptions().Configure(optionsConfigure);

        services.TryAddSingleton<ITemplateParser, DefaultTemplateParser>();
        services.TryAddSingleton<ITemplateRenderer, DefaultTemplateRenderer>();
        services.TryAddSingleton<ITemplateEngine, TemplateEngine>();

        services.TryAddEnumerable(ServiceDescriptor.Singleton<IRenderMiddleware, DefaultRenderMiddleware>());
        services.TryAddEnumerable(ServiceDescriptor.Singleton<IRenderMiddleware, EnvRenderMiddleware>());
        services.AddSingleton<IRenderMiddleware>(sp =>
        {
            var configuration = sp.GetService<IOptions<TemplateEngineOptions>>()?.Value.Configuration
                ?? sp.GetService<IConfiguration>();
            return new ConfigurationRenderMiddleware(configuration);
        });

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
