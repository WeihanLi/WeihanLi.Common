using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WeihanLi.Extensions;

namespace WeihanLi.Common.DependencyInjection;

public interface IServiceContainer : IScope, IServiceProvider
{
    IServiceContainer CreateScope();
}

internal sealed class ServiceContainer : IServiceContainer
{
    private readonly IReadOnlyList<ServiceDefinition> _services;

    private readonly ConcurrentDictionary<ServiceKey, object?> _singletonInstances;

    private readonly ConcurrentDictionary<ServiceKey, object?> _scopedInstances = new();
    private readonly ConcurrentBag<object> _transientDisposables = new();

    private class ServiceKey : IEquatable<ServiceKey>
    {
        public Type ServiceType { get; }

        public Type ImplementType { get; }

        public ServiceKey(Type serviceType, ServiceDefinition definition)
        {
            ServiceType = serviceType;
            ImplementType = definition.GetImplementType();
        }

        public bool Equals(ServiceKey? other)
        {
            return ServiceType == other?.ServiceType && ImplementType == other.ImplementType;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as ServiceKey);
        }

        public override int GetHashCode()
        {
            var key = $"{ServiceType.FullName}_{ImplementType.FullName}";
            return key.GetHashCode();
        }
    }

    private readonly bool _isRootScope;

    public ServiceContainer(IReadOnlyList<ServiceDefinition> serviceDefinitions)
    {
        _services = serviceDefinitions;

        _isRootScope = true;
        _singletonInstances = new ConcurrentDictionary<ServiceKey, object?>();
    }

    private ServiceContainer(ServiceContainer serviceContainer)
    {
        _isRootScope = false;
        _singletonInstances = serviceContainer._singletonInstances;

        _services = serviceContainer._services;
        _scopedInstances = new ConcurrentDictionary<ServiceKey, object?>();
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
            }
        }
    }

    private object? EnrichObject(object? obj)
    {
        if (obj is not null)
        {
            var type = obj.GetType();
            // PropertyInjection
            foreach (var property in CacheUtil.GetTypeProperties(type)
                .Where(x => x.IsDefined(typeof(FromServiceAttribute))))
            {
                if (property.GetValueGetter()?.Invoke(obj) == null)
                {
                    property.GetValueSetter()?.Invoke(
                        obj,
                        GetService(property.PropertyType)
                    );
                }
            }
        }
        return obj;
    }

    private object? GetServiceInstance(Type serviceType, ServiceDefinition serviceDefinition)
        => EnrichObject(GetServiceInstanceInternal(serviceType, serviceDefinition));

    private object? GetServiceInstanceInternal(Type serviceType, ServiceDefinition serviceDefinition)
    {
        if (serviceDefinition.ImplementationInstance != null)
            return serviceDefinition.ImplementationInstance;

        if (serviceDefinition.ImplementationFactory != null)
        {
            return serviceDefinition.ImplementationFactory.Invoke(this);
        }
        var implementType = (serviceDefinition.ImplementType ?? serviceType);

        if (implementType.IsInterface || implementType.IsAbstract)
        {
            throw new InvalidOperationException($"invalid service registered, serviceType: {serviceType.FullName}, implementType: {serviceDefinition.ImplementType}");
        }

        if (implementType.IsGenericType)
        {
            implementType = implementType.MakeGenericType(serviceType.GetGenericArguments());
        }

        var newFunc = CacheUtil.TypeNewFuncCache.GetOrAdd(implementType, (serviceContainer) =>
        {
            if (CacheUtil.TypeEmptyConstructorFuncCache.TryGetValue(implementType, out var emptyFunc))
            {
                return emptyFunc.Invoke();
            }

            var ctor = CacheUtil.TypeConstructorCache.GetOrAdd(implementType, t =>
            {
                var ctorInfos = t.GetConstructors(BindingFlags.Instance | BindingFlags.Public);
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
                    ctorInfo = ctorInfos.FirstOrDefault(x => x.IsDefined(typeof(ServiceConstructorAttribute)))
                        ?? ctorInfos
                        .OrderBy(_ => _.GetParameters().Length)
                        .First();
                }

                return ctorInfo;
            });
            if (ctor == null)
            {
                throw new InvalidOperationException(
                    $"service {serviceType.FullName} does not have any public constructors");
            }

            var parameters = ctor.GetParameters();
            if (parameters.Length == 0)
            {
                var func00 = Expression.Lambda<Func<object>>(Expression.New(ctor)).Compile();
                CacheUtil.TypeEmptyConstructorFuncCache.TryAdd(implementType, func00);
                return func00.Invoke();
            }

            var ctorParams = new object?[parameters.Length];
            for (var index = 0; index < parameters.Length; index++)
            {
                var param = serviceContainer.GetService(parameters[index].ParameterType);
                if (param == null && parameters[index].HasDefaultValue)
                {
                    param = parameters[index].DefaultValue;
                }
                ctorParams[index] = param;
            }

            var func = CacheUtil.TypeConstructorFuncCache.GetOrAdd(implementType, t =>
            {
                if (!CacheUtil.TypeConstructorCache.TryGetValue(t, out var ctorInfo) || ctorInfo is null)
                {
                    return null!;
                }

                var innerParameters = ctorInfo.GetParameters();
                var parameterExpression = Expression.Parameter(typeof(object[]), "arguments"); // create parameter Expression
                var argExpressions = new Expression[innerParameters.Length]; // array that will contains parameter expessions
                for (var i = 0; i < innerParameters.Length; i++)
                {
                    var indexedAccess = Expression.ArrayIndex(parameterExpression, Expression.Constant(i));

                    if (!innerParameters[i].ParameterType.IsClass)
                    {
                        // we should create local variable that will store parameter value
                        var localVariable = Expression.Variable(innerParameters[i].ParameterType, "localVariable");

                        var block = Expression.Block(new[] { localVariable },
                            Expression.IfThenElse(Expression.Equal(indexedAccess, Expression.Constant(null)),
                                Expression.Assign(localVariable, Expression.Default(innerParameters[i].ParameterType)),
                                Expression.Assign(localVariable, Expression.Convert(indexedAccess, innerParameters[i].ParameterType))
                            ),
                            localVariable
                        );

                        argExpressions[i] = block;
                    }
                    else
                    {
                        argExpressions[i] = Expression.Convert(indexedAccess, innerParameters[i].ParameterType);
                    }
                }
                // create expression that represents call to specified ctor with the specified arguments.
                var newExpression = Expression.New(ctorInfo, argExpressions);

                return Expression.Lambda<Func<object?[], object>>(newExpression, parameterExpression)
                .Compile();
            });
            return func.Invoke(ctorParams);
        });

        return newFunc?.Invoke(this);
    }

    public object? GetService(Type serviceType)
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
                            object? svc;
                            if (def.ServiceLifetime == ServiceLifetime.Singleton)
                            {
                                svc = _singletonInstances.GetOrAdd(new ServiceKey(innerServiceType, def), _ => GetServiceInstance(innerServiceType, def));
                            }
                            else if (def.ServiceLifetime == ServiceLifetime.Scoped)
                            {
                                svc = _scopedInstances.GetOrAdd(new ServiceKey(innerServiceType, def), _ => GetServiceInstance(innerServiceType, def));
                            }
                            else
                            {
                                svc = GetServiceInstance(innerServiceType, def);
                                if (svc is IDisposable)
                                {
                                    _transientDisposables.Add(svc);
                                }
                            }
                            if (svc != null)
                            {
                                list.Add(svc);
                            }
                        }

                        var methodInfo = typeof(Enumerable)
                            .GetMethod("Cast", BindingFlags.Static | BindingFlags.Public);
                        if (methodInfo != null)
                        {
                            var genericMethod = methodInfo.MakeGenericMethod(innerServiceType);
                            var castValue = genericMethod.Invoke(null, new object[] { list });
                            if (typeof(IEnumerable<>).MakeGenericType(innerServiceType) == serviceType)
                            {
                                return castValue;
                            }
                            var toArrayMethod = typeof(Enumerable).GetMethod("ToArray", BindingFlags.Static | BindingFlags.Public)
                                ?.MakeGenericMethod(innerServiceType);

                            return toArrayMethod?.Invoke(null, new[] { castValue });
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

        if (serviceDefinition.ServiceLifetime == ServiceLifetime.Scoped)
        {
            return _scopedInstances.GetOrAdd(new ServiceKey(serviceType, serviceDefinition), (t) => GetServiceInstance(t.ServiceType, serviceDefinition));
        }

        var svc1 = GetServiceInstance(serviceType, serviceDefinition);
        if (svc1 is IDisposable)
        {
            _transientDisposables.Add(svc1);
        }
        return svc1;
    }
}
