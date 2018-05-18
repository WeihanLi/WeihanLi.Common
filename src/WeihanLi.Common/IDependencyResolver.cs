using System;
using System.Collections.Generic;
using System.Linq;

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
        /// <param name="servicerType">serviceType</param>
        /// <param name="service">service</param>
        /// <returns>true if successfully get service otherwise false</returns>
        bool TryGetService(Type servicerType, out object service);

        /// <summary>
        /// GetServices
        /// </summary>
        /// <returns></returns>
        IEnumerable<object> GetServices(Type serviceType);
    }

    /// <summary>
    /// DependencyResolverExtensions
    /// </summary>
    public static class DependencyResolverExtensions
    {
        public static TService ResolveService<TService>(this IDependencyResolver dependencyResolver)
            => (TService)dependencyResolver.GetService(typeof(TService));

        public static IEnumerable<TService> ResolveServices<TService>(this IDependencyResolver dependencyResolver)
            => dependencyResolver.GetServices(typeof(TService)).Cast<TService>();
    }
}
