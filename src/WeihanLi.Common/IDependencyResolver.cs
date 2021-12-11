using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common;

/// <inheritdoc />
/// <summary>
/// IDependencyResolver
/// </summary>
public interface IDependencyResolver : IServiceProvider
{
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
    /// <summary>
    /// TryGetService
    /// </summary>
    /// <param name="dependencyResolver">dependencyResolver</param>
    /// <param name="serviceType">serviceType</param>
    /// <param name="service">service</param>
    /// <returns>true if successfully get service otherwise false</returns>
    public static bool TryGetService(this IDependencyResolver dependencyResolver, Type serviceType, out object? service)
    {
        try
        {
            service = dependencyResolver.GetService(serviceType);
            return service != null;
        }
        catch (Exception e)
        {
            service = null;
            InvokeHelper.OnInvokeException?.Invoke(e);
            return false;
        }
    }

    public static bool TryResolveService<TService>(this IDependencyResolver dependencyResolver,
        out TService? service)
    {
        var result = dependencyResolver.TryGetService(typeof(TService), out var serviceObj);
        if (result)
        {
            service = (TService)serviceObj!;
        }
        else
        {
            service = default;
        }
        return result;
    }
}
