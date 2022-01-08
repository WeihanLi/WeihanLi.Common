using WeihanLi.Common.DependencyInjection;

namespace WeihanLi.Common.Aspect;

public interface IFluentAspectsServiceContainerBuilder
{
    IServiceContainerBuilder Services { get; }
}

internal sealed class FluentAspectsServiceContainerBuilder : IFluentAspectsServiceContainerBuilder
{
    public FluentAspectsServiceContainerBuilder(IServiceContainerBuilder serviceCollection)
    {
        Services = serviceCollection;
    }

    public IServiceContainerBuilder Services { get; }
}
