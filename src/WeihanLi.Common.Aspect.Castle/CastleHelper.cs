using Castle.DynamicProxy;

namespace WeihanLi.Common.Aspect.Castle
{
    internal static class CastleHelper
    {
        public static readonly IProxyBuilder ProxyBuilder = new DefaultProxyBuilder();

        public static readonly IProxyGenerator ProxyGenerator = new ProxyGenerator(ProxyBuilder);

        public static readonly ProxyGenerationOptions ProxyGenerationOptions = new ProxyGenerationOptions()
        {
            Selector = new FluentAspectInterceptorSelector()
        };
    }
}
