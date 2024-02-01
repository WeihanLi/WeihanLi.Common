using WeihanLi.Common.DependencyInjection;

namespace WeihanLi.Common.Aspect;

public interface IFluentAspectsServiceContainerBuilder
{
    IServiceContainerBuilder Services { get; }
}

internal sealed class FluentAspectsServiceContainerBuilder(IServiceContainerBuilder serviceCollection) : IFluentAspectsServiceContainerBuilder
{
    public IServiceContainerBuilder Services { get; } = serviceCollection;
}
