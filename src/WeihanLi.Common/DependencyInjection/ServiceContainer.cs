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
        IServiceContainer Add(ServiceDefinition item);

        IServiceContainer TryAdd(ServiceDefinition item);

        IServiceContainer CreateScope();
    }

    public class ServiceContainer : IServiceContainer
    {
        private readonly ConcurrentBag<ServiceDefinition> _services;

        private readonly ConcurrentDictionary<ServiceKey, object> _singletonInstances;

        private readonly ConcurrentDictionary<ServiceKey, object> _scopedInstances;
        private ConcurrentBag<object> _transientDisposables = new ConcurrentBag<object>();

        private class ServiceKey : IEquatable<ServiceKey>
        {
            public Type ServiceType { get; }

            public Type ImplementType { get; }

            public ServiceKey(Type serviceType, ServiceDefinition definition)
            {
                ServiceType = serviceType;
                ImplementType = definition.GetImplementType();
            }

            public bool Equals(ServiceKey other)
            {
                return ServiceType == other.ServiceType && ImplementType == other.ImplementType;
            }

            public override bool Equals(object obj)
            {
                return Equals((ServiceKey)obj);
            }

            public override int GetHashCode()
            {
                var key = $"{ServiceType.FullName}_{ImplementType.FullName}";
                return key.GetHashCode();
            }
        }

        private readonly bool _isRootScope;

        public ServiceContainer()
        {
            _isRootScope = true;
            _singletonInstances = new ConcurrentDictionary<ServiceKey, object>();
            _services = new ConcurrentBag<ServiceDefinition>();
        }

        private ServiceContainer(ServiceContainer serviceContainer)
        {
            _isRootScope = false;
            _singletonInstances = serviceContainer._singletonInstances;

            _services = serviceContainer._services;
            _scopedInstances = new ConcurrentDictionary<ServiceKey, object>();
        }

        public IServiceContainer Add(ServiceDefinition item)
        {
            if (_disposed)
            {
                throw new InvalidOperationException("the service container had been disposed");
            }
            if (_services.Any(_ => _.ServiceType == item.ServiceType && _.GetImplementType() == item.GetImplementType()))
            {
                return this;
            }

            _services.Add(item);
            return this;
        }

        public IServiceContainer TryAdd(ServiceDefinition item)
        {
            if (_disposed)
            {
                throw new InvalidOperationException("the service container had been disposed");
            }
            if (_services.Any(_ => _.ServiceType == item.ServiceType))
            {
                return this;
            }
            _services.Add(item);
            return this;
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
                    _transientDisposables = null;
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
                    _transientDisposables = null;
                }
            }
        }

        private readonly ConcurrentDictionary<Type, Func<object>> _newFuncCache =
            new ConcurrentDictionary<Type, Func<object>>();

        private readonly ConcurrentDictionary<Type, Func<object[], object>> _newFuncCache2 =
            new ConcurrentDictionary<Type, Func<object[], object>>();

        private readonly ConcurrentDictionary<Type, ConstructorInfo> _serviceCtorCache =
            new ConcurrentDictionary<Type, ConstructorInfo>();

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

            var ctor = _serviceCtorCache.GetOrAdd(implementType, (t) =>
            {
                var ctorInfos = implementType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
                if (ctorInfos.Length == 0)
                {
                    return null;
                }

                ConstructorInfo ctorInfo;
                if (ctorInfos.Length == 1)
                {
                    ctorInfo = ctorInfos[0];
                }
                else
                {
                    // TODO: try find best ctor
                    ctorInfo = ctorInfos
                        .OrderBy(_ => _.GetParameters().Length)
                        .First();
                }

                return ctorInfo;
            });
            if (null == ctor)
            {
                throw new InvalidOperationException($"service {serviceType.FullName} does not have any public constructors");
            }

            var parameters = ctor.GetParameters();

            if (parameters.Length == 0)
            {
                var func = _newFuncCache.GetOrAdd(implementType, (t) => Expression.Lambda<Func<object>>(Expression.New(ctor)).Compile());
                return func.Invoke();
            }
            else
            {
                // 优化创建带参数的构造方法的执行
                var func = _newFuncCache2.GetOrAdd(implementType, (t) =>
                {
                    var parameterExpression = Expression.Parameter(typeof(object[]), "arguments"); // create parameter Expression
                    var argExpressions = new Expression[parameters.Length]; // array that will contains parameter expessions
                    for (var i = 0; i < parameters.Length; i++)
                    {
                        var indexedAccess = Expression.ArrayIndex(parameterExpression, Expression.Constant(i));

                        if (!parameters[i].ParameterType.IsClass) // check if parameter is a value type
                        {
                            var localVariable = Expression.Variable(parameters[i].ParameterType, "localVariable"); // if so - we should create local variable that will store paraameter value

                            var block = Expression.Block(new[] { localVariable },
                                Expression.IfThenElse(Expression.Equal(indexedAccess, Expression.Constant(null)),
                                    Expression.Assign(localVariable, Expression.Default(parameters[i].ParameterType)),
                                    Expression.Assign(localVariable, Expression.Convert(indexedAccess, parameters[i].ParameterType))
                                ),
                                localVariable
                            );

                            argExpressions[i] = block;
                        }
                        else
                        {
                            argExpressions[i] = Expression.Convert(indexedAccess, parameters[i].ParameterType);
                        }
                    }
                    var newExpression = Expression.New(ctor, argExpressions); // create expression that represents call to specified ctor with the specified arguments.

                    return Expression.Lambda<Func<object[], object>>(newExpression, parameterExpression)
                        .Compile();
                });

                var ctorParams = new object[parameters.Length];
                for (var index = 0; index < parameters.Length; index++)
                {
                    var param = GetService(parameters[index].ParameterType);
                    if (param == null && parameters[index].HasDefaultValue)
                    {
                        param = parameters[index].DefaultValue;
                    }
                    ctorParams[index] = param;
                }
                return func(ctorParams);
            }
        }

        public object GetService(Type serviceType)
        {
            if (_disposed)
            {
                throw new InvalidOperationException($"can not get scope service from a disposed scope, serviceType: {serviceType.FullName}");
            }

            var serviceDefinition = _services.LastOrDefault(_ => _.ServiceType == serviceType);
            if (null == serviceDefinition)
            {
                if (serviceType.IsGenericType)
                {
                    var genericType = serviceType.GetGenericTypeDefinition();
                    serviceDefinition = _services.LastOrDefault(_ => _.ServiceType == genericType);
                    if (null == serviceDefinition)
                    {
                        var innerServiceType = serviceType.GetGenericArguments().First();
                        if (typeof(IEnumerable<>).MakeGenericType(innerServiceType)
                            .IsAssignableFrom(serviceType))
                        {
                            var innerRegType = innerServiceType;
                            if (innerServiceType.IsGenericType)
                            {
                                innerRegType = innerServiceType.GetGenericTypeDefinition();
                            }
                            //
                            var list = new List<object>(4);
                            foreach (var def in _services.Where(_ => _.ServiceType == innerRegType))
                            {
                                object svc;
                                if (def.ServiceLifetime == ServiceLifetime.Singleton)
                                {
                                    svc = _singletonInstances.GetOrAdd(new ServiceKey(innerServiceType, def), (t) => GetServiceInstance(innerServiceType, def));
                                }
                                else if (def.ServiceLifetime == ServiceLifetime.Scoped)
                                {
                                    svc = _scopedInstances.GetOrAdd(new ServiceKey(innerServiceType, def), (t) => GetServiceInstance(innerServiceType, def));
                                }
                                else
                                {
                                    svc = GetServiceInstance(innerServiceType, def);
                                    if (svc is IDisposable)
                                    {
                                        _transientDisposables.Add(svc);
                                    }
                                }
                                if (null != svc)
                                {
                                    list.Add(svc);
                                }
                            }

                            var methodInfo = typeof(Enumerable)
                                .GetMethod("Cast", BindingFlags.Static | BindingFlags.Public);
                            if (methodInfo != null)
                            {
                                var genericMethod = methodInfo.MakeGenericMethod(innerServiceType);
                                var castedValue = genericMethod.Invoke(null, new object[] { list });
                                if (typeof(IEnumerable<>).MakeGenericType(innerServiceType) == serviceType)
                                {
                                    return castedValue;
                                }
                                var toArrayMethod = typeof(Enumerable).GetMethod("ToArray", BindingFlags.Static | BindingFlags.Public)
                                    ?.MakeGenericMethod(innerServiceType);

                                return toArrayMethod?.Invoke(null, new object[] { castedValue });
                            }
                            return list;
                        }

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
                return _singletonInstances.GetOrAdd(new ServiceKey(serviceType, serviceDefinition), (t) => GetServiceInstance(t.ServiceType, serviceDefinition));
            }
            else if (serviceDefinition.ServiceLifetime == ServiceLifetime.Scoped)
            {
                return _scopedInstances.GetOrAdd(new ServiceKey(serviceType, serviceDefinition), (t) => GetServiceInstance(t.ServiceType, serviceDefinition));
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
