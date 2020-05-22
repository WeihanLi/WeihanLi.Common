# WeihanLi.Common.Aspect.AspectCore [![WeihanLi.Common.Aspect.AspectCore](https://img.shields.io/nuget/v/WeihanLi.Common.Aspect.AspectCore.svg)](https://www.nuget.org/packages/WeihanLi.Common.Aspect.AspectCore/)

## Intro

`AspectCore` extensions, FluentAspect AspectCore extensions

## Use

``` csharp
services.AddFluentAspects(options =>
    {
        options.NoInterceptPropertyGetter<IFly>(f => f.Name);

        options.InterceptAll()
            .With<LogInterceptor>()
            ;
        options.InterceptMethod<DbContext>(x => x.Name == nameof(DbContext.SaveChanges)
                                                || x.Name == nameof(DbContext.SaveChangesAsync))
            .With<DbContextSaveInterceptor>()
            ;
        options.InterceptMethod<IFly>(f => f.Fly())
            .With<LogInterceptor>();
        options.InterceptType<IFly>()
            .With<LogInterceptor>();

        options
            .WithProperty("TraceId", "121212")
            ;
    })
    .UseAspectCoreProxy()
    ;
```
