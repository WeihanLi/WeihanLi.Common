using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeihanLi.Common
{
    /// <inheritdoc />
    /// <summary>
    /// IDependencyResolver
    /// </summary>
    public interface IDependencyResolver : IServiceProvider
    {
        /// <summary>
        /// TryGetService
        /// </summary>
        /// <param name="serviceType">serviceType</param>
        /// <param name="service">service</param>
        /// <returns>true if successfully get service otherwise false</returns>
        bool TryGetService(Type serviceType, out object service);

        /// <summary>
        /// GetServices
        /// </summary>
        /// <returns></returns>
        IEnumerable<object> GetServices(Type serviceType);

        /// <summary>
        /// Invoke action via get a service instance internal
        /// </summary>
        /// <typeparam name="TService">service type</typeparam>
        /// <param name="action">action</param>
        bool TryInvokeService<TService>(Action<TService> action);

        Task<bool> TryInvokeServiceAsync<TService>(Func<TService, Task> action);
    }

    /// <summary>
    /// DependencyResolverExtensions
    /// </summary>
    public static class DependencyResolverExtensions
    {
        public static bool TryResolveService<TService>(this IDependencyResolver dependencyResolver,
            out TService service)
        {
            var result = dependencyResolver.TryGetService(typeof(TService), out var serviceObj);
            service = (TService)serviceObj;
            return result;
        }

        public static TService ResolveService<TService>(this IDependencyResolver dependencyResolver)
            => (TService)dependencyResolver.GetService(typeof(TService));

        public static IEnumerable<TService> ResolveServices<TService>(this IDependencyResolver dependencyResolver)
            => dependencyResolver.GetServices(typeof(TService)).Cast<TService>();
    }
}
