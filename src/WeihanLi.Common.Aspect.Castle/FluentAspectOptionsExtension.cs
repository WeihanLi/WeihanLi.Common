namespace WeihanLi.Common.Aspect.Castle;

public static class FluentAspectOptionsExtension
{
    public static FluentAspectOptions UseCastleProxy(this FluentAspectOptions options)
    {
        options.ProxyFactory = CastleProxyFactory.Instance;

        return options;
    }
}
