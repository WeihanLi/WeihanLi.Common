using JetBrains.Annotations;
using System;
using System.Linq;
using System.Reflection;
using WeihanLi.Extensions;

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
#if NET45
            if (null == assemblies || assemblies.Length == 0)
            {
                if (System.Web.Hosting.HostingEnvironment.IsHosted)
                {
                    assemblies = System.Web.Compilation.BuildManager.GetReferencedAssemblies()
                                            .Cast<Assembly>().ToArray();
                }
            }
#endif

            if (null == assemblies || assemblies.Length == 0)
            {
                assemblies = AppDomain.CurrentDomain.GetAssemblies();
            }

            foreach (var type in assemblies.WhereNotNull().SelectMany(ass => ass.GetTypes())
                .Where(t => t.IsClass && !t.IsAbstract && typeof(ServiceContainerModule).IsAssignableFrom(t))
            )
            {
                try
                {
                    if (Activator.CreateInstance(type) is ServiceContainerModule module)
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
