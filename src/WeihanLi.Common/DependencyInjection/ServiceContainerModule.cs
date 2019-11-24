namespace WeihanLi.Common.DependencyInjection
{
    public abstract class ServiceContainerModule
    {
        public abstract void ConfigureServices(IServiceContainerBuilder serviceContainerBuilder);
    }
}
