using Microsoft.Extensions.DependencyInjection;

namespace WeihanLi.Common.Aspect;

public interface IFluentAspectsBuilder
{
    IServiceCollection Services { get; }
}

internal sealed class FluentAspectsBuilder(IServiceCollection serviceCollection) : IFluentAspectsBuilder
{
    public IServiceCollection Services { get; } = serviceCollection;
}
