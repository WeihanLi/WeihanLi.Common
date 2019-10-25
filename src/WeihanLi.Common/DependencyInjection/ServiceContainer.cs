using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace WeihanLi.Common.DependencyInjection
{
    public interface IServiceContainer : IScope, IServiceProvider
    {
        void Add(ServiceDefinition item);

        void Clear();

        IServiceContainer CreateScope();
    }

    public class ServiceContainer : IServiceContainer
    {
        internal readonly List<ServiceDefinition> _services;

        private readonly ConcurrentDictionary<Type, object> _singletonInstances;

        private readonly ConcurrentDictionary<Type, object> _scopedInstances;
        private readonly List<object> _transientDisposables = new List<object>();

        private readonly bool _isRootScope;

        public ServiceContainer()
        {
            _isRootScope = true;
            _singletonInstances = new ConcurrentDictionary<Type, object>();
            _services = new List<ServiceDefinition>();
        }

        internal ServiceContainer(ServiceContainer serviceContainer)
        {
            _isRootScope = false;
            _singletonInstances = serviceContainer._singletonInstances;
            _services = serviceContainer._services;
            _scopedInstances = new ConcurrentDictionary<Type, object>();
        }

        public void Add(ServiceDefinition item)
        {
            _services.Add(item);
        }

        public void Clear()
        {
            _services.Clear();
        }

        public IServiceContainer CreateScope()
        {
            return new ServiceContainer(this);
        }

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            if (_isRootScope)
            {
                lock (_singletonInstances)
                {
                    if (_disposed)
                    {
                        return;
                    }

                    _disposed = true;
                    foreach (var instance in _singletonInstances.Values)
                    {
                        (instance as IDisposable)?.Dispose();
                    }

                    foreach (var o in _transientDisposables)
                    {
                        (o as IDisposable)?.Dispose();
                    }

                    _singletonInstances.Clear();
                    _transientDisposables.Clear();
                }
            }
            else
            {
                lock (_scopedInstances)
                {
                    if (_disposed)
                    {
                        return;
                    }

                    _disposed = true;
                    foreach (var instance in _scopedInstances.Values)
                    {
                        (instance as IDisposable)?.Dispose();
                    }

                    foreach (var o in _transientDisposables)
                    {
                        (o as IDisposable)?.Dispose();
                    }

                    _scopedInstances.Clear();
                    _transientDisposables.Clear();
                }
            }
        }

        private object GetServiceInstance(Type serviceType, ServiceDefinition serviceDefinition)
        {
            if (serviceDefinition.ImplementationInstance != null)
                return serviceDefinition.ImplementationInstance;

            if (serviceDefinition.ImplementationFactory != null)
                return serviceDefinition.ImplementationFactory.Invoke(this);

            var implementType = (serviceDefinition.ImplementType ?? serviceType);

            if (implementType.IsInterface || implementType.IsAbstract)
            {
                throw new InvalidOperationException($"invalid service registered, serviceType: {serviceType.FullName}, implementType: {serviceDefinition.ImplementType}");
            }

            if (implementType.IsGenericType)
            {
                implementType = implementType.MakeGenericType(serviceType.GetGenericArguments());
            }

            var ctorInfos = implementType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
            if (ctorInfos.Length == 0)
            {
                throw new InvalidOperationException($"service {serviceType.FullName} does not have any public constructors");
            }

            ConstructorInfo ctor;
            if (ctorInfos.Length == 1)
            {
                ctor = ctorInfos[0];
            }
            else
            {
                // TODO: try find best ctor
                ctor = ctorInfos
                    .OrderBy(_ => _.GetParameters().Length)
                    .First();
            }

            var parameters = ctor.GetParameters();
            if (parameters.Length == 0)
            {
                // TODO: cache New Func
                return Expression.Lambda<Func<object>>(Expression.New(ctor)).Compile().Invoke();
            }
            else
            {
                var ctorParams = new object[parameters.Length];
                for (var index = 0; index < parameters.Length; index++)
                {
                    var parameter = parameters[index];
                    var param = GetService(parameter.ParameterType);
                    if (param == null && parameter.HasDefaultValue)
                    {
                        param = parameter.DefaultValue;
                    }

                    ctorParams[index] = param;
                }
                return Expression.Lambda<Func<object>>(Expression.New(ctor, ctorParams.Select(Expression.Constant))).Compile().Invoke();
            }
        }

        public object GetService(Type serviceType)
        {
            var serviceDefinition = _services.LastOrDefault(_ => _.ServiceType == serviceType);
            if (null == serviceDefinition)
            {
                //
                if (serviceType.IsGenericType)
                {
                    var genericType = serviceType.GetGenericTypeDefinition();
                    serviceDefinition = _services.LastOrDefault(_ => _.ServiceType == genericType);
                    if (null == serviceDefinition)
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }

            if (_isRootScope && serviceDefinition.ServiceLifetime == ServiceLifetime.Scoped)
            {
                throw new InvalidOperationException($"can not get scope service from the root scope, serviceType: {serviceType.FullName}");
            }

            if (serviceDefinition.ServiceLifetime == ServiceLifetime.Singleton)
            {
                var svc = _singletonInstances.GetOrAdd(serviceType, (t) => GetServiceInstance(t, serviceDefinition));
                return svc;
            }
            else if (serviceDefinition.ServiceLifetime == ServiceLifetime.Scoped)
            {
                var svc = _scopedInstances.GetOrAdd(serviceType, (t) => GetServiceInstance(t, serviceDefinition));
                return svc;
            }
            else
            {
                var svc = GetServiceInstance(serviceType, serviceDefinition);
                if (svc is IDisposable)
                {
                    _transientDisposables.Add(svc);
                }
                return svc;
            }
        }
    }
}
