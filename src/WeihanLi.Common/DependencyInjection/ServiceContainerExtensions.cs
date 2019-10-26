using JetBrains.Annotations;

namespace WeihanLi.Common.DependencyInjection
{
    public static partial class ServiceContainerExtensions
    {
        public static IServiceContainer AddSingleton<TService>([NotNull]this IServiceContainer serviceContainer, [NotNull]TService service)
        {
            serviceContainer.Add(new ServiceDefinition(service, typeof(TService)));
            return serviceContainer;
        }
    }
}
