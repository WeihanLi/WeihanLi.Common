﻿using AspNetCoreSample.Events;
using WeihanLi.Common;
using WeihanLi.Common.Event;

namespace AspNetCoreSample;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration.ReplacePlaceholders();
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllersWithViews()
            ;

        services.AddEvents()
            .AddEventHandler<PageViewEvent, PageViewEventHandler>()
            ;

        services.AddSingleton<IEventPublisher, EventQueuePublisher>();
        services.AddHostedService<EventConsumer>();

        // TestReplaceHolder
        var abc = Configuration["TestSetting2:Setting2"];
        Console.WriteLine(abc);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Init service locator
        DependencyResolver.SetDependencyResolver(app.ApplicationServices);

        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
        }

        // pageView middleware
        app.Use((context, next) =>
        {
            var eventPublisher = context.RequestServices
                .GetRequiredService<IEventPublisher>();
            eventPublisher.Publish(new PageViewEvent()
            {
                Path = context.Request.Path.Value ?? "",
            });

            return next();
        });
        app.UseHttpLogging();
        app.UseRouting();

        app.UseEndpoints(endpoint =>
        {
            endpoint.MapControllers();
            endpoint.MapDefaultControllerRoute();
        });
    }
}
