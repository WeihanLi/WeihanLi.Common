using System;

namespace WeihanLi.Common.DependencyInjection
{
    public class ServiceDefinition
    {
        public ServiceLifetime ServiceLifetime { get; }

        public Type ImplementType { get; }

        public Type ServiceType { get; }

        public object ImplementationInstance { get; }

        public Func<IServiceProvider, object> ImplementationFactory { get; }

        public Type GetImplementType()
        {
            if (ImplementationInstance != null)
                return ImplementationInstance.GetType();

            if (ImplementationFactory != null)
                return ImplementationFactory.Method.ReturnType;

            if (ImplementType != null)
                return ImplementType;

            return ServiceType;
        }

        public ServiceDefinition(object instance, Type serviceType)
        {
            ImplementationInstance = instance;
            ServiceType = serviceType;
            ServiceLifetime = ServiceLifetime.Singleton;
        }

        public ServiceDefinition(Type serviceType, ServiceLifetime serviceLifetime) : this(serviceType, serviceType, serviceLifetime)
        {
        }

        public ServiceDefinition(Type serviceType, Type implementType, ServiceLifetime serviceLifetime)
        {
            ServiceType = serviceType;
            ImplementType = implementType ?? serviceType;
            ServiceLifetime = serviceLifetime;
        }

        public ServiceDefinition(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime serviceLifetime)
        {
            ServiceType = serviceType;
            ImplementationFactory = factory;
            ServiceLifetime = serviceLifetime;
        }

        public static ServiceDefinition Singleton<TService>(Func<IServiceProvider, object> factory)
        {
            return new(typeof(TService), factory, ServiceLifetime.Singleton);
        }

        public static ServiceDefinition Singleton<TService, TServiceImplement>() where TServiceImplement : TService
        {
            return new(typeof(TService), typeof(TServiceImplement), ServiceLifetime.Singleton);
        }

        public static ServiceDefinition Singleton<TService>()
        {
            return new(typeof(TService), ServiceLifetime.Singleton);
        }

        public static ServiceDefinition Scoped<TService>(Func<IServiceProvider, object> factory)
        {
            return new(typeof(TService), factory, ServiceLifetime.Scoped);
        }

        public static ServiceDefinition Scoped<TService, TServiceImplement>() where TServiceImplement : TService
        {
            return new(typeof(TService), typeof(TServiceImplement), ServiceLifetime.Scoped);
        }

        public static ServiceDefinition Scoped<TService>()
        {
            return new(typeof(TService), ServiceLifetime.Scoped);
        }

        public static ServiceDefinition Transient<TService>(Func<IServiceProvider, object> factory)
        {
            return new(typeof(TService), factory, ServiceLifetime.Transient);
        }

        public static ServiceDefinition Transient<TService>()
        {
            return new(typeof(TService), ServiceLifetime.Transient);
        }

        public static ServiceDefinition Transient<TService, TServiceImplement>() where TServiceImplement : TService
        {
            return new(typeof(TService), typeof(TServiceImplement), ServiceLifetime.Transient);
        }
    }
}
