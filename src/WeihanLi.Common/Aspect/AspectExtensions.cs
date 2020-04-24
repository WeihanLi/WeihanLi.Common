using System;
using System.Linq;
using System.Reflection;

namespace WeihanLi.Common.Aspect
{
    public static class AspectExtensions
    {
        public static TInterface CreateInterfaceProxy<TInterface>(this IProxyFactory proxyGenerator)
        {
            var type = typeof(TInterface);
            if (!type.IsInterface)
            {
                throw new InvalidOperationException($"the Type {type.FullName} is not interface");
            }
            return proxyGenerator.CreateProxy<TInterface>();
        }

        public static TInterface CreateInterfaceProxy<TInterface, TImplement>(this IProxyFactory proxyGenerator) where TImplement : TInterface
        {
            var type = typeof(TInterface);
            if (!type.IsInterface)
            {
                throw new InvalidOperationException($"the Type {type.FullName} is not interface");
            }
            return proxyGenerator.CreateProxy<TInterface, TImplement>();
        }

        public static TClass CreateClassProxy<TClass>(this IProxyFactory proxyGenerator) where TClass : class
        {
            var type = typeof(TClass);
            if (!type.IsClass)
            {
                throw new InvalidOperationException($"the Type {type.FullName} is not class");
            }
            return proxyGenerator.CreateProxy<TClass>();
        }

        public static TClass CreateClassProxy<TClass, TImplement>(this IProxyFactory proxyGenerator) where TImplement : TClass
        {
            var type = typeof(TClass);
            if (!type.IsClass)
            {
                throw new InvalidOperationException($"the Type {type.FullName} is not class");
            }
            return proxyGenerator.CreateProxy<TClass, TImplement>();
        }

        public static MethodInfo GetBaseMethod(this MethodInfo currentMethod)
        {
            if (null == currentMethod?.DeclaringType?.BaseType)
                return null;

            var parameterTypes = currentMethod.GetParameters().Select(x => x.ParameterType).ToArray();
            var baseMethod = currentMethod.DeclaringType.BaseType.GetMethod(currentMethod.Name, parameterTypes);
            return baseMethod;
        }
    }
}
