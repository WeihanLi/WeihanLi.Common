using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeihanLi.Common.DependencyInjection;

#if NETSTANDARD

using Microsoft.Extensions.DependencyInjection;

#endif

namespace WeihanLi.Common
{
    /// <summary>
    /// DependencyResolver
    /// </summary>
    public static class DependencyResolver
    {
        public static IDependencyResolver Current { get; private set; } = new DefaultDependencyResolver();

        /// <summary>
        /// locker
        /// </summary>
        private static readonly object _lock = new object();

        public static TService ResolveService<TService>() => Current.ResolveService<TService>();

        public static IEnumerable<TService> ResolveServices<TService>() => Current.ResolveServices<TService>();

        public static bool TryInvoke<TService>(Action<TService> action) => Current.TryInvokeService(action);

        public static Task<bool> TryInvokeAsync<TService>(Func<TService, Task> action) => Current.TryInvokeServiceAsync(action);

        public static void SetDependencyResolver([NotNull] IDependencyResolver dependencyResolver)
        {
            lock (_lock)
            {
                Current = dependencyResolver;
            }
        }

        public static void SetDependencyResolver([NotNull] IServiceContainer serviceContainer) => SetDependencyResolver(new ServiceContainerDependencyResolver(serviceContainer));

        public static void SetDependencyResolver([NotNull] IServiceProvider serviceProvider) => SetDependencyResolver(serviceProvider.GetService);

        public static void SetDependencyResolver([NotNull] Func<Type, object> getServiceFunc) => SetDependencyResolver(getServiceFunc, serviceType => (IEnumerable<object>)getServiceFunc(typeof(IEnumerable<>).MakeGenericType(serviceType)));

        public static void SetDependencyResolver([NotNull] Func<Type, object> getServiceFunc, [NotNull] Func<Type, IEnumerable<object>> getServicesFunc) => SetDependencyResolver(new DelegateBasedDependencyResolver(getServiceFunc, getServicesFunc));

#if NETSTANDARD

        public static void SetDependencyResolver(IServiceCollection services) => SetDependencyResolver(new ServiceCollectionDependencyResolver(services));

        public static void SetDependencyResolver(IServiceCollection services, ServiceProviderOptions serviceProviderOptions) => SetDependencyResolver(new ServiceCollectionDependencyResolver(services, serviceProviderOptions));

        private sealed class ServiceCollectionDependencyResolver : IDependencyResolver
        {
            private readonly IServiceProvider _serviceProvider;

            public ServiceCollectionDependencyResolver(IServiceCollection services) : this(services, null)
            {
            }

            public ServiceCollectionDependencyResolver(IServiceCollection services, ServiceProviderOptions serviceProviderOptions) : this(services.BuildServiceProvider(serviceProviderOptions ?? new ServiceProviderOptions()))
            {
            }

            private ServiceCollectionDependencyResolver(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public object GetService(Type serviceType)
            {
                return _serviceProvider.GetService(serviceType);
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                return _serviceProvider.GetServices(serviceType);
            }

            public bool TryInvokeService<TService>(Action<TService> action)
            {
                if (action == null)
                {
                    return false;
                }
                using (var scope = _serviceProvider.CreateScope())
                {
                    var svc = (TService)scope.ServiceProvider.GetService(typeof(TService));
                    if (svc == null)
                    {
                        return false;
                    }
                    action.Invoke(svc);
                    return true;
                }
            }

            public async Task<bool> TryInvokeServiceAsync<TService>(Func<TService, Task> action)
            {
                if (action == null)
                {
                    return false;
                }
                using (var scope = _serviceProvider.CreateScope())
                {
                    var svc = (TService)scope.ServiceProvider.GetService(typeof(TService));
                    if (svc == null)
                    {
                        return false;
                    }
                    await action.Invoke(svc);
                    return true;
                }
            }
        }

#endif

        private class DefaultDependencyResolver : IDependencyResolver
        {
            public object GetService(Type serviceType)
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

            public bool TryInvokeService<TService>(Action<TService> action)
            {
                var service = (TService)GetService(typeof(TService));
                if (null == service || action == null)
                {
                    return false;
                }
                action.Invoke(service);
                return true;
            }

            public async Task<bool> TryInvokeServiceAsync<TService>(Func<TService, Task> action)
            {
                var service = (TService)GetService(typeof(TService));
                if (null == service || action == null)
                {
                    return false;
                }
                await action.Invoke(service);
                return true;
            }
        }

        private class DelegateBasedDependencyResolver : IDependencyResolver
        {
            private readonly Func<Type, object> _getService;
            private readonly Func<Type, IEnumerable<object>> _getServices;

            public DelegateBasedDependencyResolver(Func<Type, object> getService, Func<Type, IEnumerable<object>> getServices)
            {
                _getService = getService;
                _getServices = getServices;
            }

            public object GetService(Type serviceType)
            => _getService(serviceType);

            public IEnumerable<object> GetServices(Type serviceType)
                => _getServices(serviceType);

            public bool TryInvokeService<TService>(Action<TService> action)
            {
                var service = (TService)GetService(typeof(TService));
                if (null == service || action == null)
                {
                    return false;
                }
                action.Invoke(service);
                return true;
            }

            public async Task<bool> TryInvokeServiceAsync<TService>(Func<TService, Task> action)
            {
                var service = (TService)GetService(typeof(TService));
                if (null == service || action == null)
                {
                    return false;
                }
                await action.Invoke(service);
                return true;
            }
        }

        private class ServiceContainerDependencyResolver : IDependencyResolver
        {
            private readonly IServiceContainer _rootContainer;

            public ServiceContainerDependencyResolver(IServiceContainer serviceContainer)
            {
                _rootContainer = serviceContainer;
            }

            public object GetService(Type serviceType)
            {
                return _rootContainer.GetService(serviceType);
            }

            public IEnumerable<object> GetServices(Type serviceType)
            {
                return (IEnumerable<object>)_rootContainer.GetService(typeof(IEnumerable<>).MakeGenericType(serviceType));
            }

            public bool TryInvokeService<TService>(Action<TService> action)
            {
                if (action == null)
                {
                    return false;
                }
                using (var scope = _rootContainer.CreateScope())
                {
                    var svc = (TService)scope.GetService(typeof(TService));
                    if (svc == null)
                    {
                        return false;
                    }
                    action.Invoke(svc);
                    return true;
                }
            }

            public async Task<bool> TryInvokeServiceAsync<TService>(Func<TService, Task> action)
            {
                if (action == null)
                {
                    return false;
                }
                using (var scope = _rootContainer.CreateScope())
                {
                    var svc = (TService)scope.GetService(typeof(TService));
                    if (svc == null)
                    {
                        return false;
                    }
                    await action.Invoke(svc);
                    return true;
                }
            }
        }
    }
}
