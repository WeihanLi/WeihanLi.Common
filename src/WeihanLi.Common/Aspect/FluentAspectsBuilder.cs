using Microsoft.Extensions.DependencyInjection;

namespace WeihanLi.Common.Aspect;

public interface IFluentAspectsBuilder
{
    IServiceCollection Services { get; }
}

internal sealed class FluentAspectsBuilder : IFluentAspectsBuilder
{
    public FluentAspectsBuilder(IServiceCollection serviceCollection)
    {
        Services = serviceCollection;
    }

    public IServiceCollection Services { get; }
}
