using WeihanLi.Common.Helpers;

namespace WeihanLi.Common;

/// <summary>
/// Resolves services from the application's dependency container.
/// </summary>
public interface IDependencyResolver : IServiceProvider
{
    /// <summary>
    /// Gets all registered service instances for the specified service type.
    /// </summary>
    /// <param name="serviceType">The service type to resolve.</param>
    /// <returns>The resolved service instances.</returns>
    IEnumerable<object> GetServices(Type serviceType);

    /// <summary>
    /// Resolves a service and invokes the specified action when the service is available.
    /// </summary>
    /// <typeparam name="TService">The service type to resolve.</typeparam>
    /// <param name="action">The action to invoke with the resolved service instance.</param>
    /// <returns><see langword="true"/> when the service is resolved and the action is invoked; otherwise, <see langword="false"/>.</returns>
    bool TryInvokeService<TService>(Action<TService> action);

    /// <summary>
    /// Resolves a service and invokes the specified asynchronous action when the service is available.
    /// </summary>
    /// <typeparam name="TService">The service type to resolve.</typeparam>
    /// <param name="action">The asynchronous action to invoke with the resolved service instance.</param>
    /// <returns><see langword="true"/> when the service is resolved and the action is invoked; otherwise, <see langword="false"/>.</returns>
    Task<bool> TryInvokeServiceAsync<TService>(Func<TService, Task> action);
}

/// <summary>
/// Extension methods for <see cref="IDependencyResolver"/>.
/// </summary>
public static class DependencyResolverExtensions
{
    /// <summary>
    /// Tries to resolve a service of the specified type.
    /// </summary>
    /// <param name="dependencyResolver">The dependency resolver.</param>
    /// <param name="serviceType">The service type to resolve.</param>
    /// <param name="service">When this method returns, contains the resolved service instance if resolution succeeded; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when the service is resolved; otherwise, <see langword="false"/>.</returns>
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

    /// <summary>
    /// Tries to resolve a service of the specified generic type.
    /// </summary>
    /// <typeparam name="TService">The service type to resolve.</typeparam>
    /// <param name="dependencyResolver">The dependency resolver.</param>
    /// <param name="service">When this method returns, contains the resolved service instance if resolution succeeded; otherwise, <see langword="null"/>.</param>
    /// <returns><see langword="true"/> when the service is resolved; otherwise, <see langword="false"/>.</returns>
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
