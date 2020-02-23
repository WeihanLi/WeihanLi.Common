using JetBrains.Annotations;
using System;
using System.Linq;
using System.Reflection;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.DependencyInjection
{
    public static partial class ServiceContainerBuilderExtensions
    {
        public static IServiceContainerBuilder AddSingleton<TService>([NotNull]this IServiceContainerBuilder serviceContainerBuilder, [NotNull]TService service)
        {
            serviceContainerBuilder.Add(new ServiceDefinition(service, typeof(TService)));
            return serviceContainerBuilder;
        }

        public static IServiceContainerBuilder RegisterAssemblyModules(
            [NotNull] this IServiceContainerBuilder serviceContainerBuilder, params Assembly[] assemblies)
        {
            if (null == assemblies || assemblies.Length == 0)
            {
                assemblies = ReflectHelper.GetAssemblies();
            }

            foreach (var type in assemblies.
                SelectMany(ass => ass.GetExportedTypes())
                .Where(t => t.IsClass && !t.IsAbstract && typeof(IServiceContainerModule).IsAssignableFrom(t))
            )
            {
                try
                {
                    if (Activator.CreateInstance(type) is IServiceContainerModule module)
                    {
                        module.ConfigureServices(serviceContainerBuilder);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            return serviceContainerBuilder;
        }
    }
}
