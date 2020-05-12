using AspectCore.Configuration;
using AspectCore.DynamicProxy;

namespace WeihanLi.Common.Aspect.AspectCore
{
    internal class AspectCoreHelper
    {
        public static readonly IProxyGenerator ProxyGenerator;

        static AspectCoreHelper()
        {
            var proxyGeneratorBuilder = new ProxyGeneratorBuilder();
            proxyGeneratorBuilder.Configure(configuration =>
            {
                configuration.Interceptors.AddTyped<FluentAspectInterceptor>();
            });
            ProxyGenerator = proxyGeneratorBuilder.Build();
        }
    }
}
