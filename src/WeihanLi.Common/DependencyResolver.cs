using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

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

        public static void SetDependencyResolver(IServiceProvider serviceProvider)
        {
            SetDependencyResolver(new ServiceCollectionDependencyResolver(serviceProvider));
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

            public ServiceCollectionDependencyResolver(IServiceCollection services)
            {
                if (null == services)
                {
                    throw new ArgumentNullException(nameof(services));
                }
                _serviceProvider = services.BuildServiceProvider();
            }

            public ServiceCollectionDependencyResolver(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            }

            public object GetService(Type serviceType)
            {
                return _serviceProvider.GetService(serviceType);
            }

            public bool TryGetService(Type serviceType, out object service)
            {
                try
                {
                    service = _serviceProvider.GetService(serviceType);
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
        }

#endif
    }
}
