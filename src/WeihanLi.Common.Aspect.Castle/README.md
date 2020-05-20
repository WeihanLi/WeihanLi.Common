# WeihanLi.Common.Aspect.Castle [![WeihanLi.Common.Aspect.Castle](https://img.shields.io/nuget/v/WeihanLi.Common.Aspect.Castle.svg)](https://www.nuget.org/packages/WeihanLi.Common.Aspect.Castle/)

## Intro

`Castle` extensions, FluentAspect Castle extensions

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
    .UseCastleProxy()
    // .UseAspectCoreProxy()
    ;
```
