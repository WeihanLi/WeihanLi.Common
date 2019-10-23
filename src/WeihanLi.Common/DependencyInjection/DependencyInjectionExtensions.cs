using JetBrains.Annotations;
using System;
using System.Collections.Generic;

namespace WeihanLi.Common
{
    public static class DependencyInjectionExtensions
    {
        /// <summary>
        /// ResolveService
        /// </summary>
        /// <typeparam name="TService">TService</typeparam>
        /// <param name="serviceProvider">serviceProvider</param>
        /// <returns></returns>
        public static TService ResolveService<TService>([NotNull]this IServiceProvider serviceProvider)
       => (TService)serviceProvider.GetService(typeof(TService));

        /// <summary>
        /// Resolve services
        /// </summary>
        /// <typeparam name="TService">TService</typeparam>
        /// <param name="serviceProvider">serviceProvider</param>
        /// <returns></returns>
        public static IEnumerable<TService> ResolveServices<TService>([NotNull]this IServiceProvider serviceProvider)
            => (IEnumerable<TService>)serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(typeof(TService)));
    }
}
