using Microsoft.Extensions.DependencyInjection;

namespace WeihanLi.Common.Aspect.Castle
{
    public static class FluentAspectBuilderExtensions
    {
        public static IFluentAspectBuilder UseCastleProxy(this IFluentAspectBuilder builder)
        {
            builder.Services.AddTransient<IProxyFactory, CastleProxyFactory>();

            FluentAspects.AspectOptions.ProxyFactory = CastleProxyFactory.Instance;

            return builder;
        }
    }
}
