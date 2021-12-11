using AspectCore.Configuration;
using AspectCore.DynamicProxy;

namespace WeihanLi.Common.Aspect.AspectCore;

internal static class AspectCoreHelper
{
    public static readonly IProxyGenerator ProxyGenerator;

    static AspectCoreHelper()
    {
        var proxyGeneratorBuilder = new FluentAspectProxyGeneratorBuilder();
        proxyGeneratorBuilder.Configure(configuration =>
        {
            configuration.Interceptors.AddTyped<AspectCoreFluentAspectInterceptor>();
        });
        ProxyGenerator = proxyGeneratorBuilder.Build();
    }
}
