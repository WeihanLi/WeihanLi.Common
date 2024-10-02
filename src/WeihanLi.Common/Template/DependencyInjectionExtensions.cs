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
    public static ITemplateEngineServiceBuilder AddTemplateEngine(
        this IServiceCollection services, 
        Action<TemplateEngineOptions>? optionsConfigure = null
        )
    {
        Guard.NotNull(services);
        if (services.Any(x => x.ServiceType == typeof(ITemplateEngine)))
            throw new InvalidOperationException("Template engine services had been registered");

        if (optionsConfigure != null)
            services.AddOptions().Configure(optionsConfigure);

        services.TryAddSingleton<ITemplateEngine, TemplateEngine>();
        services.TryAddSingleton<ITemplateParser, DefaultTemplateParser>();
        services.TryAddSingleton<ITemplateRenderer, DefaultTemplateRenderer>();
        
        var serviceBuilder = new TemplateEngineServiceBuilder(services);
        
        serviceBuilder.AddPipe<TextFormatTemplatePipe>()
            .AddPipe<UpperCaseTemplatePipe>()
            .AddPipe<LowerCaseTemplatePipe>()
            .AddPipe<TitleCaseTemplatePipe>()
            ;

        serviceBuilder.AddRenderMiddleware(sp =>
            {
                var pipes = sp.GetServices<ITemplatePipe>().ToDictionary(p => p.Name, p => p);
                return new DefaultRenderMiddleware(pipes);
            })
            .AddRenderMiddleware<EnvRenderMiddleware>()
            .AddRenderMiddleware(sp =>
            {
                var configuration = sp.GetService<IOptions<TemplateEngineOptions>>()?.Value.Configuration
                    ?? sp.GetService<IConfiguration>();
                return new ConfigurationRenderMiddleware(configuration);
            })
            ;

        services.AddSingleton(sp =>
        {
            var pipelineBuilder = PipelineBuilder.CreateAsync<TemplateRenderContext>();
            foreach (var middleware in sp.GetServices<IRenderMiddleware>())
            {
                pipelineBuilder.UseMiddleware(middleware);
            }
            return pipelineBuilder.Build();
        });

        return serviceBuilder;
    }

    public static ITemplateEngineServiceBuilder AddRenderMiddleware<TMiddleware>(
        this ITemplateEngineServiceBuilder serviceBuilder, 
        Func<IServiceProvider, TMiddleware>? middlewareFactory = null
        ) where TMiddleware : class, IRenderMiddleware
    {
        var serviceDescriptor = middlewareFactory is null
                ? ServiceDescriptor.Singleton<IRenderMiddleware, TMiddleware>()
                : ServiceDescriptor.Singleton<IRenderMiddleware>(middlewareFactory)
            ;
        serviceBuilder.Services.Add(serviceDescriptor);
        return serviceBuilder;
    }
    
    public static ITemplateEngineServiceBuilder AddPipe<TPipe>
        (this ITemplateEngineServiceBuilder serviceBuilder) 
        where TPipe : class, ITemplatePipe
    {
        serviceBuilder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ITemplatePipe, TPipe>());
        return serviceBuilder;
    }
}
