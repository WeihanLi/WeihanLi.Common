namespace WeihanLi.Common.Aspect;

public sealed class FluentAspectOptions
{
    public readonly Dictionary<Func<IInvocation, bool>, IInterceptionConfiguration> InterceptionConfigurations = new();
    private IInterceptorResolver _interceptorResolver = FluentConfigInterceptorResolver.Instance;

    public HashSet<Func<IInvocation, bool>> NoInterceptionConfigurations { get; } = new();

    public IInterceptorResolver InterceptorResolver
    {
        get => _interceptorResolver;
        set => _interceptorResolver = Guard.NotNull(value);
    }

    public HashSet<IInvocationEnricher> Enrichers { get; } = new();

    public IProxyFactory ProxyFactory { get; set; } = DefaultProxyFactory.Instance;
}
