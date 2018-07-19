using System;
using System.Collections.Generic;
using System.Linq;

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

        public static void SetDependencyResolver(IDependencyResolver dependencyResolver)
        {
            lock (_lock)
            {
                Current = dependencyResolver;
            }
        }

        public static void SetDependencyResolver(Func<Type, object> getServiceFunc, Func<Type, IEnumerable<object>> getServicesFunc) => SetDependencyResolver(new DelegateBasedDependencyResolver(getServiceFunc, getServicesFunc));

        public static void SetDependencyResolver(IServiceProvider serviceProvider) => SetDependencyResolver(serviceProvider.GetService,
            serviceType => (IEnumerable<object>)serviceProvider.GetService(typeof(IEnumerable<>).MakeGenericType(serviceType)));

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
    }
}
