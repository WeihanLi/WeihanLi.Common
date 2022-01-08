namespace WeihanLi.Common.DependencyInjection;

public interface IServiceContainerModule
{
    void ConfigureServices(IServiceContainerBuilder serviceContainerBuilder);
}

public abstract class ServiceContainerModule : IServiceContainerModule
{
    public abstract void ConfigureServices(IServiceContainerBuilder serviceContainerBuilder);
}
