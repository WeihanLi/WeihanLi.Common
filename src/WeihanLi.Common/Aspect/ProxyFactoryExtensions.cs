using System;

namespace WeihanLi.Common.Aspect
{
    public static class ProxyFactoryExtensions
    {
        public static TInterface CreateInterfaceProxy<TInterface>(this IProxyFactory proxyGenerator)
            where TInterface : class
        {
            var type = typeof(TInterface);
            if (!type.IsInterface)
            {
                throw new InvalidOperationException($"the Type {type.FullName} is not interface");
            }
            return proxyGenerator.CreateProxy<TInterface>();
        }

        public static TInterface CreateInterfaceProxy<TInterface, TImplement>(this IProxyFactory proxyGenerator, params object[] arguments) where TImplement : TInterface
            where TInterface : class
        {
            var type = typeof(TInterface);
            if (!type.IsInterface)
            {
                throw new InvalidOperationException($"the Type {type.FullName} is not interface");
            }
            return proxyGenerator.CreateProxy<TInterface, TImplement>(arguments);
        }

        public static TClass CreateClassProxy<TClass>(this IProxyFactory proxyGenerator, params object[] arguments) where TClass : class
        {
            var type = typeof(TClass);
            if (!type.IsClass)
            {
                throw new InvalidOperationException($"the Type {type.FullName} is not class");
            }
            return proxyGenerator.CreateProxy<TClass>(arguments);
        }

        public static TClass CreateClassProxy<TClass, TImplement>(this IProxyFactory proxyGenerator, params object[] arguments) where TImplement : TClass
            where TClass : class
        {
            var type = typeof(TClass);
            if (!type.IsClass)
            {
                throw new InvalidOperationException($"the Type {type.FullName} is not class");
            }
            return proxyGenerator.CreateProxy<TClass, TImplement>(arguments);
        }

        #region CreateProxy

        public static object CreateProxy(this IProxyFactory proxyFactory, Type serviceType, params object[] arguments)
        {
            return proxyFactory.CreateProxy(serviceType, arguments);
        }

        public static TService CreateProxy<TService>(this IProxyFactory proxyFactory, params object[] arguments) where TService : class
        {
            return (TService)proxyFactory.CreateProxy(typeof(TService), arguments);
        }

        public static TService CreateProxy<TService, TImplement>(this IProxyFactory proxyFactory, params object[] arguments)
            where TImplement : TService
            where TService : class
        {
            return (TService)proxyFactory.CreateProxy(typeof(TService), typeof(TImplement), arguments);
        }

        #endregion CreateProxy

        #region CreateProxyWithTarget

        public static TInterface CreateInterfaceProxy<TInterface, TImplement>(this IProxyFactory proxyGenerator, TImplement implement) where TImplement : TInterface
            where TInterface : class
        {
            var type = typeof(TInterface);
            if (!type.IsInterface)
            {
                throw new InvalidOperationException($"the Type {type.FullName} is not interface");
            }
            return proxyGenerator.CreateProxyWithTarget<TInterface>(implement);
        }

        public static TClass CreateClassProxy<TClass, TImplement>(this IProxyFactory proxyGenerator, TImplement implement) where TImplement : TClass
            where TClass : class
        {
            var type = typeof(TClass);
            if (!type.IsClass)
            {
                throw new InvalidOperationException($"the Type {type.FullName} is not class");
            }
            return proxyGenerator.CreateProxyWithTarget<TClass>(implement);
        }

        public static TService CreateProxyWithTarget<TService, TImplement>(this IProxyFactory proxyFactory, TImplement target)
            where TImplement : TService
            where TService : class
        {
            return (TService)proxyFactory.CreateProxyWithTarget(typeof(TService), target);
        }

        public static TService CreateProxyWithTarget<TService>(this IProxyFactory proxyFactory, object target)
            where TService : class
        {
            return (TService)proxyFactory.CreateProxyWithTarget(typeof(TService), target);
        }

        #endregion CreateProxyWithTarget
    }
}
