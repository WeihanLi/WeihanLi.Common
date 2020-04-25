using Microsoft.Extensions.DependencyInjection;

namespace WeihanLi.Common.Aspect.Castle
{
    public static class FluentAspectBuilderExtensions
    {
        public static IFluentAspectBuilder UseCastleProxyFactory(this IFluentAspectBuilder builder)
        {
            builder.Services.AddSingleton<IProxyFactory, CastleProxyFactory>();
            return builder;
        }

        public static IFluentAspectBuilder UseCastleProxyTypeFactory(this IFluentAspectBuilder builder)
        {
            builder.Services.AddSingleton<IProxyTypeFactory, CastleProxyTypeFactory>();
            return builder;
        }
    }
}
