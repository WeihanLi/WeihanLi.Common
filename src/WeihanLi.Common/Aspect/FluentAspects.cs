namespace WeihanLi.Common.Aspect;

public static class FluentAspects
{
    public static readonly FluentAspectOptions AspectOptions;

    static FluentAspects()
    {
        AspectOptions = new FluentAspectOptions();
        // register built-in ignore interceptors
        AspectOptions
            .NoInterceptType(t => t.Namespace?.StartsWith("WeihanLi.Common.Aspect") == true)
            ;
        // register built-in necessary interceptors
        AspectOptions.InterceptAll()
            .With<TryInvokeInterceptor>();
        AspectOptions.InterceptMethod<IDisposable>(m => m.Dispose())
            .With<DisposableInterceptor>();
    }

    public static void Configure(Action<FluentAspectOptions> optionsAction)
    {
        Guard.NotNull(optionsAction, nameof(optionsAction)).Invoke(AspectOptions);
    }
}
