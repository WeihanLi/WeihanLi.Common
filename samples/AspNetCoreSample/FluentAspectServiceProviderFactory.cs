using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using WeihanLi.Common.Aspect;

namespace AspNetCoreSample
{
    public class FluentAspectServiceProviderFactory : IServiceProviderFactory<IServiceCollection>
    {
        private readonly Action<FluentAspectOptions> _optionsAction;
        private readonly Func<Type, bool> _ignoreTypesPredict;

        public FluentAspectServiceProviderFactory(
            Action<FluentAspectOptions> optionsAction,
            Func<Type, bool> ignoreTypesPredict
            )
        {
            _optionsAction = optionsAction;
            _ignoreTypesPredict = ignoreTypesPredict;
        }

        public IServiceCollection CreateBuilder(IServiceCollection services)
        {
            return services;
        }

        public IServiceProvider CreateServiceProvider(IServiceCollection containerBuilder)
        {
            return containerBuilder.BuildFluentAspectsProvider(_optionsAction, _ignoreTypesPredict);
        }
    }

    public static class HostBuilderExtensions
    {
        public static IHostBuilder UseFluentAspectServiceProviderFactory(this IHostBuilder hostBuilder,
            Action<FluentAspectOptions> optionsAction, Func<Type, bool> ignoreTypesPredict = null)
        {
            if (ignoreTypesPredict == null)
            {
                ignoreTypesPredict = t =>
                    t.Namespace?.StartsWith("Microsoft.") == true
                    || t.Namespace?.StartsWith("System.") == true
                    ;
            }
            hostBuilder.UseServiceProviderFactory(
                new FluentAspectServiceProviderFactory(optionsAction, ignoreTypesPredict)
                );
            return hostBuilder;
        }
    }
}
