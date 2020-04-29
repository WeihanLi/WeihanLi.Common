using Microsoft.Extensions.DependencyInjection;

namespace WeihanLi.Common.Aspect
{
    public interface IFluentAspectBuilder
    {
        IServiceCollection Services { get; }
    }

    internal class FluentAspectBuilder : IFluentAspectBuilder
    {
        public FluentAspectBuilder(IServiceCollection serviceCollection)
        {
            Services = serviceCollection;
        }

        public IServiceCollection Services { get; }
    }

    public static class FluentAspectBuilderExtensions
    {
    }
}
