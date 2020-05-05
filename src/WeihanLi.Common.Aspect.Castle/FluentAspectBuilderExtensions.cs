using Microsoft.Extensions.DependencyInjection;

namespace WeihanLi.Common.Aspect.Castle
{
    public static class FluentAspectBuilderExtensions
    {
        public static IFluentAspectBuilder UseCastle(this IFluentAspectBuilder builder)
        {
            builder.Services.AddTransient<IProxyTypeFactory, CastleProxyTypeFactory>();
            builder.Services.AddTransient<IProxyFactory, CastleProxyFactory>();

            return builder;
        }
    }
}
