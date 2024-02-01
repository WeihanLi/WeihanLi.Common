using System.Linq.Expressions;
using WeihanLi.Common.Aspect;
using WeihanLi.Extensions;

namespace AspNetCoreSample;

internal sealed class FluentAspectsServiceProviderFactory(
    Action<FluentAspectOptions>? optionsAction,
    Action<IFluentAspectsBuilder>? aspectBuildAction,
    Expression<Func<Type, bool>>? ignoreTypesPredict
        ) : IServiceProviderFactory<IServiceCollection>
{
    private readonly Action<FluentAspectOptions>? _optionsAction = optionsAction;
    private readonly Action<IFluentAspectsBuilder>? _aspectBuildAction = aspectBuildAction;
    private readonly Expression<Func<Type, bool>>? _ignoreTypesPredict = ignoreTypesPredict;

    public IServiceCollection CreateBuilder(IServiceCollection services)
    {
        return services;
    }

    public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
    {
        return containerBuilder.BuildFluentAspectsProvider(_optionsAction, _aspectBuildAction, _ignoreTypesPredict);
    }
}

public static class HostBuilderExtensions
{
    public static IHostBuilder UseFluentAspectsServiceProviderFactory(this IHostBuilder hostBuilder,
        Action<FluentAspectOptions>? optionsAction,
        Action<IFluentAspectsBuilder>? aspectBuildAction = null,
        Expression<Func<Type, bool>>? ignoreTypesPredict = null)
    {
        ignoreTypesPredict ??= t =>
                t.HasNamespace() &&
                (t.Namespace!.StartsWith("Microsoft.")
                || t.Namespace.StartsWith("System.")
                )
                ;
        hostBuilder.UseServiceProviderFactory(
            new FluentAspectsServiceProviderFactory(optionsAction, aspectBuildAction, ignoreTypesPredict)
            );
        return hostBuilder;
    }
}
