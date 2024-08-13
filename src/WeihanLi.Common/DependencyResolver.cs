using Microsoft.Extensions.DependencyInjection;
using WeihanLi.Common.DependencyInjection;

namespace WeihanLi.Common;

/// <summary>
/// DependencyResolver
/// Service locator pattern
/// </summary>
public static class DependencyResolver
{
    private static readonly Lock _lock = new();
    public static IDependencyResolver Current { get; private set; } = new DefaultDependencyResolver();

    public static TService? ResolveService<TService>() => Current.ResolveService<TService>();

    public static TService ResolveRequiredService<TService>() => Current.ResolveRequiredService<TService>();

    public static IEnumerable<TService> ResolveServices<TService>() => Current.ResolveServices<TService>();

    public static bool TryInvoke<TService>(Action<TService> action) => Current.TryInvokeService(action);

    public static Task<bool> TryInvokeAsync<TService>(Func<TService, Task> action) => Current.TryInvokeServiceAsync(action);

    public static void SetDependencyResolver(IDependencyResolver dependencyResolver)
    {
        lock (_lock)
        {
            Current = dependencyResolver;
        }
    }

    public static void SetDependencyResolver(IServiceContainer serviceContainer) => SetDependencyResolver(new ServiceContainerDependencyResolver(serviceContainer));

    public static void SetDependencyResolver(IServiceProvider serviceProvider)
    {
        Guard.NotNull(serviceProvider);
        if (serviceProvider is ServiceProvider microServiceProvider)
            SetDependencyResolver(new ServiceProviderDependencyResolver(microServiceProvider));
        else
            SetDependencyResolver(serviceProvider.GetService);
    }

    public static void SetDependencyResolver(Func<Type, object?> getServiceFunc) => SetDependencyResolver(getServiceFunc, serviceType => (IEnumerable<object>)Guard.NotNull(getServiceFunc(typeof(IEnumerable<>).MakeGenericType(serviceType))));

    public static void SetDependencyResolver(Func<Type, object?> getServiceFunc, Func<Type, IEnumerable<object>> getServicesFunc) => SetDependencyResolver(new DelegateBasedDependencyResolver(getServiceFunc, getServicesFunc));

    public static void SetDependencyResolver(IServiceCollection services) => SetDependencyResolver(new ServiceProviderDependencyResolver(services.BuildServiceProvider()));

    private sealed class ServiceProviderDependencyResolver(ServiceProvider serviceProvider) : IDependencyResolver
    {
        public object? GetService(Type serviceType)
        {
            return serviceProvider.GetService(serviceType);
        }

        [RequiresUnreferencedCode("Calls WeihanLi.Common.DependencyInjectionExtensions.GetServices(Type)")]
        public IEnumerable<object> GetServices(Type serviceType)
        {
            return serviceProvider.GetServices(serviceType);
        }

        public bool TryInvokeService<TService>(Action<TService> action)
        {
            Guard.NotNull(action, nameof(action));
            using var scope = serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetService<TService>();
            if (service is null)
                return false;
            action.Invoke(service);
            return true;
        }

        public async Task<bool> TryInvokeServiceAsync<TService>(Func<TService, Task> action)
        {
            Guard.NotNull(action, nameof(action));
            await using var scope = serviceProvider.CreateAsyncScope();
            var service = scope.ServiceProvider.GetService<TService>();
            if (service is null)
                return false;
            await action.Invoke(service);
            return true;
        }
    }

    private sealed class DefaultDependencyResolver : IDependencyResolver
    {
        public object? GetService([DynamicallyAccessedMembers((DynamicallyAccessedMemberTypes.PublicParameterlessConstructor))] Type serviceType)
        {
            // Since attempting to create an instance of an interface or an abstract type results in an exception, immediately return null
            // to improve performance and the debugging experience with first-chance exceptions enabled.
            if (serviceType.IsInterface || serviceType.IsAbstract)
            {
                return null;
            }
            try
            {
                return Activator.CreateInstance(serviceType);
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<object> GetServices(Type serviceType) => Enumerable.Empty<object>();

        public bool TryInvokeService<TService>(Action<TService>? action)
        {
            var service = GetService(typeof(TService));
            if (null == service || action == null)
            {
                return false;
            }
            action.Invoke((TService)service);
            return true;
        }

        public async Task<bool> TryInvokeServiceAsync<TService>(Func<TService, Task>? action)
        {
            var service = GetService(typeof(TService));
            if (null == service || action == null)
            {
                return false;
            }
            await action.Invoke((TService)service);
            return true;
        }
    }

    private sealed class DelegateBasedDependencyResolver(Func<Type, object?> getService, Func<Type, IEnumerable<object>> getServices) : IDependencyResolver
    {
        private readonly Func<Type, object?> _getService = Guard.NotNull(getService);
        private readonly Func<Type, IEnumerable<object>> _getServices = Guard.NotNull(getServices);

        public object? GetService(Type serviceType)
        => _getService(serviceType);

        public IEnumerable<object> GetServices(Type serviceType)
            => _getServices(serviceType);

        public bool TryInvokeService<TService>(Action<TService>? action)
        {
            var svc = GetService(typeof(TService));
            if (action != null && svc is TService service)
            {
                action.Invoke(service);
                return true;
            }
            return false;
        }

        public async Task<bool> TryInvokeServiceAsync<TService>(Func<TService, Task>? action)
        {
            var svc = GetService(typeof(TService));
            if (action != null && svc is TService service)
            {
                await action.Invoke(service);
                return true;
            }
            return false;
        }
    }

    private sealed class ServiceContainerDependencyResolver(IServiceContainer serviceContainer) : IDependencyResolver
    {
        public object? GetService(Type serviceType)
        {
            return serviceContainer.GetService(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return (IEnumerable<object>)Guard.NotNull(serviceContainer.GetService(typeof(IEnumerable<>).MakeGenericType(serviceType)));
        }

        public bool TryInvokeService<TService>(Action<TService> action)
        {
            Guard.NotNull(action, nameof(action));

            using var scope = serviceContainer.CreateScope();
            var svc = scope.GetService(typeof(TService));
            if (svc is TService service)
            {
                action.Invoke(service);
                return true;
            }
            return false;
        }

        public async Task<bool> TryInvokeServiceAsync<TService>(Func<TService, Task> action)
        {
            Guard.NotNull(action, nameof(action));
            using var scope = serviceContainer.CreateScope();
            var svc = scope.GetService(typeof(TService));
            if (svc is TService service)
            {
                await action.Invoke(service);
                return true;
            }
            return false;
        }
    }
}
