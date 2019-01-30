using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

#if NETSTANDARD2_0

using Microsoft.Extensions.DependencyInjection;

#endif

namespace WeihanLi.Common
{
    /// <summary>
    /// DependencyResolver
    /// </summary>
    public static class DependencyResolver
    {
        public static IDependencyResolver Current { get; private set; }

        /// <summary>
        /// locker
        /// </summary>
        private static readonly object _lock = new object();

        static DependencyResolver()
        {
            Current = new DefaultDependencyResolver();
        }

#if NETSTANDARD2_0

        public static void SetDependencyResolver(IServiceCollection services)
        {
            SetDependencyResolver(new ServiceCollectionDependencyResolver(services));
        }

#else
        public static void SetDependencyResolver(IServiceProvider serviceProvider) => SetDependencyResolver(serviceProvider.GetService,
            serviceType => (IEnumerable<object>)serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(serviceType)));
#endif

        public static void SetDependencyResolver(IDependencyResolver dependencyResolver)
        {
            lock (_lock)
            {
                Current = dependencyResolver;
            }
        }

        public static void SetDependencyResolver(Func<Type, object> getServiceFunc, Func<Type, IEnumerable<object>> getServicesFunc) => SetDependencyResolver(new DelegateBasedDependencyResolver(getServiceFunc, getServicesFunc));

        public static void SetDependencyResolver(Func<Type, object> getServiceFunc) => SetDependencyResolver(getServiceFunc, serviceType => (IEnumerable<object>)getServiceFunc(typeof(IEnumerable<>).MakeGenericType(serviceType)));

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

            public IEnumerable<object> GetServices(Type serviceType)
            => Enumerable.Empty<object>();

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

            public bool TryGetService(Type serviceType, out object service)
            {
                service = GetService(serviceType);
                return serviceType != null;
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

            public object GetService(Type type)
            => _getService(type);

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

            public bool TryGetService(Type serviceType, out object service)
            {
                try
                {
                    service = _getService(serviceType);
                    return true;
                }
                catch
                {
                    service = null;
                    return false;
                }
            }
        }

#if NETSTANDARD2_0

        private class ServiceCollectionDependencyResolver : IDependencyResolver
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly IServiceCollection _services;

            public ServiceCollectionDependencyResolver(IServiceCollection services)
            {
                _services = services ?? throw new ArgumentNullException(nameof(services));
                _serviceProvider = services.BuildServiceProvider();
            }

            public object GetService(Type serviceType)
            {
                var serviceDescriptor = _services.FirstOrDefault(_ => _.ServiceType == serviceType);
                if (serviceDescriptor == null)
                {
                    throw new InvalidOperationException($"service type {serviceType.FullName} has not been registered");
                }

                if (serviceDescriptor.Lifetime == ServiceLifetime.Scoped) // 这样返回的话，如果是一个 IDisposable 对象的话，返回的是一个已经被 dispose 掉的对象
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        return scope.ServiceProvider.GetService(serviceType);
                    }
                }
                return _serviceProvider.GetService(serviceType);
            }

            public bool TryGetService(Type serviceType, out object service)
            {
                try
                {
                    service = GetService(serviceType);
                    return true;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    service = null;
                    return false;
                }
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
                var serviceType = typeof(TService);
                var serviceDescriptor = _services.FirstOrDefault(_ => _.ServiceType == serviceType);
                if (serviceDescriptor == null)
                {
                    return false;
                }

                if (serviceDescriptor.Lifetime == ServiceLifetime.Scoped) // 这样返回的话，如果是一个 IDisposable 对象的话，返回的是一个已经被 dispose 掉的对象
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var svc = (TService)scope.ServiceProvider.GetService(serviceType);
                        if (svc == null)
                        {
                            return false;
                        }
                        action.Invoke(svc);
                        return true;
                    }
                }
                var service = (TService)GetService(typeof(TService));
                if (null == service)
                {
                    return false;
                }
                action.Invoke(service);
                return true;
            }

            public async Task<bool> TryInvokeServiceAsync<TService>(Func<TService, Task> action)
            {
                if (action == null)
                {
                    return false;
                }
                var serviceType = typeof(TService);
                var serviceDescriptor = _services.FirstOrDefault(_ => _.ServiceType == serviceType);
                if (serviceDescriptor == null)
                {
                    return false;
                }

                if (serviceDescriptor.Lifetime == ServiceLifetime.Scoped) // 这样返回的话，如果是一个 IDisposable 对象的话，返回的是一个已经被 dispose 掉的对象
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var svc = (TService)scope.ServiceProvider.GetService(serviceType);
                        if (svc == null)
                        {
                            return false;
                        }
                        await action.Invoke(svc);
                        return true;
                    }
                }
                var service = (TService)GetService(typeof(TService));
                if (null == service)
                {
                    return false;
                }
                await action.Invoke(service);
                return true;
            }
        }

#endif
    }
}
