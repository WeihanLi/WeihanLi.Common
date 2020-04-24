using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Aspect
{
    internal static class ProxyUtils
    {
        private const string ProxyAssemblyName = "WeihanLi.Aop.DynamicGenerated";
        private static readonly ModuleBuilder _moduleBuilder;
        private static readonly ConcurrentDictionary<string, Type> _proxyTypes = new ConcurrentDictionary<string, Type>();

        private static readonly HashSet<string> _ignoredMethodNames = new HashSet<string>();

        static ProxyUtils()
        {
            _ignoredMethodNames.Add(nameof(ProxyAssemblyName.ToString));
            _ignoredMethodNames.Add(nameof(ProxyAssemblyName.GetType));
            _ignoredMethodNames.Add(nameof(ProxyAssemblyName.GetHashCode));
            _ignoredMethodNames.Add(nameof(ProxyAssemblyName.Equals));

            var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(ProxyAssemblyName), AssemblyBuilderAccess.Run);
            _moduleBuilder = asmBuilder.DefineDynamicModule("Default");
        }

#if NETSTANDARD2_0

        private static Type CreateType(this TypeBuilder typeBuilder)
        {
            return typeBuilder.CreateTypeInfo()?.AsType();
        }

#endif

        public static Type CreateInterfaceProxy(Type interfaceType)
        {
            if (null == interfaceType)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }
            if (!interfaceType.IsInterface)
            {
                throw new InvalidOperationException($"{interfaceType.FullName} is not an interface");
            }
            var proxyTypeName = $"{ProxyAssemblyName}.{interfaceType.FullName}";
            var type = _proxyTypes.GetOrAdd(proxyTypeName, name =>
            {
                var typeBuilder = _moduleBuilder.DefineType(proxyTypeName, TypeAttributes.Public, typeof(object), new[] { interfaceType });
                typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

                var methods = interfaceType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                foreach (var method in methods)
                {
                    var methodParameterTypes = method.GetParameters()
                        .Select(p => p.ParameterType)
                        .ToArray();
                    var methodBuilder = typeBuilder.DefineMethod(method.Name
                        , MethodAttributes.Public | MethodAttributes.Virtual,
                        method.CallingConvention,
                        method.ReturnType,
                        methodParameterTypes
                        );
                    foreach (var customAttribute in method.CustomAttributes)
                    {
                        methodBuilder.SetCustomAttribute(DefineCustomAttribute(customAttribute));
                    }
                    typeBuilder.DefineMethodOverride(methodBuilder, method);

                    var il = methodBuilder.GetILGenerator();

                    var localReturnValue = il.DeclareReturnValue(method.ReturnType);
                    var localCurrentMethod = il.DeclareLocal(typeof(MethodInfo));
                    var localParameters = il.DeclareLocal(typeof(object[]));

                    // var currentMethod = MethodBase.GetCurrentMethod();
                    il.Call(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod)));
                    il.EmitCastToType(typeof(MethodBase), typeof(MethodInfo));
                    il.Emit(OpCodes.Stloc, localCurrentMethod);

                    // var parameters = new[] {a, b, c};
                    il.Emit(OpCodes.Ldc_I4, methodParameterTypes.Length);
                    il.Emit(OpCodes.Newarr, typeof(object));
                    if (methodParameterTypes.Length > 0)
                    {
                        for (var i = 0; i < methodParameterTypes.Length; i++)
                        {
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Ldc_I4, i);
                            il.Emit(OpCodes.Ldarg, i + 1);
                            if (methodParameterTypes[i].IsValueType)
                            {
                                il.Emit(OpCodes.Box, methodParameterTypes[i].UnderlyingSystemType);
                            }

                            il.Emit(OpCodes.Stelem_Ref);
                        }
                    }
                    il.Emit(OpCodes.Stloc, localParameters);

                    // var aspectInvocation = new AspectInvocation(method, this, parameters);
                    var localAspectInvocation = il.DeclareLocal(typeof(MethodInvocation));
                    il.Emit(OpCodes.Ldloc, localCurrentMethod);
                    il.EmitNull();
                    il.Emit(OpCodes.Ldarg_0);
                    il.EmitNull();
                    il.Emit(OpCodes.Ldloc, localParameters);

                    il.New(typeof(MethodInvocation).GetConstructors()[0]);
                    il.Emit(OpCodes.Stloc, localAspectInvocation);

                    // AspectDelegate.InvokeAspectDelegate(invocation);
                    il.Emit(OpCodes.Ldloc, localAspectInvocation);
                    var invokeAspectDelegateMethod =
                        typeof(AspectDelegate).GetMethod(nameof(AspectDelegate.Invoke), new[] { typeof(IInvocation) });
                    il.Call(invokeAspectDelegateMethod);
                    il.Emit(OpCodes.Nop);

                    if (method.ReturnType != typeof(void))
                    {
                        // load return value
                        il.Emit(OpCodes.Ldloc, localAspectInvocation);
                        var getMethod = typeof(MethodInvocation).GetProperty("ReturnValue").GetGetMethod();
                        il.EmitCall(OpCodes.Callvirt, getMethod, Type.EmptyTypes);

                        if (method.ReturnType != typeof(object))
                        {
                            il.EmitCastToType(typeof(object), method.ReturnType);
                        }

                        il.Emit(OpCodes.Stloc, localReturnValue);
                        il.Emit(OpCodes.Ldloc, localReturnValue);
                    }

                    il.Emit(OpCodes.Ret);
                }

                return typeBuilder.CreateType();
            });
            return type;
        }

        public static Type CreateInterfaceProxy(Type interfaceType, Type implementType)
        {
            if (null == interfaceType)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }
            if (!interfaceType.IsInterface)
            {
                throw new InvalidOperationException($"{interfaceType.FullName} is not an interface");
            }

            if (null == implementType)
                return CreateInterfaceProxy(interfaceType);

            if (implementType.IsSealed)
                throw new InvalidOperationException("the implementType is sealed");

            //
            var proxyTypeName = $"{ProxyAssemblyName}.{interfaceType.FullName}.{implementType.FullName}";

            var type = _proxyTypes.GetOrAdd(proxyTypeName, name =>
            {
                var typeBuilder = _moduleBuilder.DefineType(proxyTypeName, implementType.Attributes, implementType, new[] { interfaceType });

                foreach (var constructor in implementType.GetConstructors())
                {
                    var constructorTypes = constructor.GetParameters().Select(o => o.ParameterType).ToArray();
                    var constructorBuilder = typeBuilder.DefineConstructor(
                        MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName,
                        CallingConventions.Standard,
                        constructorTypes);
                    foreach (var customAttribute in constructor.CustomAttributes)
                    {
                        constructorBuilder.SetCustomAttribute(DefineCustomAttribute(customAttribute));
                    }
                    var il = constructorBuilder.GetILGenerator();
                    il.Emit(OpCodes.Ldarg_0);

                    for (var i = 0; i < constructorTypes.Length; i++)
                    {
                        il.Emit(OpCodes.Ldarg, i + 1);
                    }

                    il.Call(constructor);
                    il.Emit(OpCodes.Nop);
                    il.Emit(OpCodes.Ret);
                }

                var methods = interfaceType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                foreach (var method in methods)
                {
                    var methodParameterTypes = method.GetParameters()
                        .Select(p => p.ParameterType)
                        .ToArray();
                    var methodBuilder = typeBuilder.DefineMethod(method.Name,
                        MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig |
                          MethodAttributes.Virtual | MethodAttributes.NewSlot,
                        method.CallingConvention,
                        method.ReturnType,
                        methodParameterTypes
                        );
                    foreach (var customAttribute in method.CustomAttributes)
                    {
                        methodBuilder.SetCustomAttribute(DefineCustomAttribute(customAttribute));
                    }
                    typeBuilder.DefineMethodOverride(methodBuilder, method);

                    var il = methodBuilder.GetILGenerator();

                    var localReturnValue = il.DeclareReturnValue(method.ReturnType);
                    var localCurrentMethod = il.DeclareLocal(typeof(MethodInfo));
                    var localMethodBase = il.DeclareLocal(typeof(MethodInfo));
                    var localParameters = il.DeclareLocal(typeof(object[]));
                    var localTarget = il.DeclareLocal(typeof(object));

                    // var currentMethod = MethodBase.GetCurrentMethod();
                    il.Call(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod)));
                    il.EmitCastToType(typeof(MethodBase), typeof(MethodInfo));
                    il.Emit(OpCodes.Stloc, localCurrentMethod);

                    il.Emit(OpCodes.Ldloc, localCurrentMethod);
                    il.Call(typeof(AspectExtensions).GetMethod(nameof(AspectExtensions.GetBaseMethod)));
                    il.Emit(OpCodes.Stloc, localMethodBase);

                    // var parameters = new[] {a, b, c};
                    il.Emit(OpCodes.Ldc_I4, methodParameterTypes.Length);
                    il.Emit(OpCodes.Newarr, typeof(object));
                    if (methodParameterTypes.Length > 0)
                    {
                        for (var i = 0; i < methodParameterTypes.Length; i++)
                        {
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Ldc_I4, i);
                            il.Emit(OpCodes.Ldarg, i + 1);
                            if (methodParameterTypes[i].IsValueType)
                            {
                                il.Emit(OpCodes.Box, methodParameterTypes[i].UnderlyingSystemType);
                            }

                            il.Emit(OpCodes.Stelem_Ref);
                        }
                    }
                    il.Emit(OpCodes.Stloc, localParameters);

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Stloc, localTarget);

                    // var invocation = new MethodInvocation(method, methodBase, this, parameters);
                    var localAspectInvocation = il.DeclareLocal(typeof(MethodInvocation));
                    il.Emit(OpCodes.Ldloc, localCurrentMethod);
                    il.Emit(OpCodes.Ldloc, localMethodBase);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldloc, localTarget);
                    il.Emit(OpCodes.Ldloc, localParameters);

                    il.New(typeof(MethodInvocation).GetConstructors()[0]);
                    il.Emit(OpCodes.Stloc, localAspectInvocation);

                    // AspectDelegate.InvokeAspectDelegate(invocation);
                    il.Emit(OpCodes.Ldloc, localAspectInvocation);
                    var invokeAspectDelegateMethod =
                        typeof(AspectDelegate).GetMethod(nameof(AspectDelegate.Invoke), new[] { typeof(IInvocation) });
                    il.Call(invokeAspectDelegateMethod);
                    il.Emit(OpCodes.Nop);

                    if (method.ReturnType != typeof(void))
                    {
                        // load return value
                        il.Emit(OpCodes.Ldloc, localAspectInvocation);
                        var getMethod = typeof(MethodInvocation).GetProperty("ReturnValue").GetGetMethod();
                        il.EmitCall(OpCodes.Callvirt, getMethod, Type.EmptyTypes);

                        if (method.ReturnType.IsValueType)
                        {
                            il.EmitCastToType(typeof(object), method.ReturnType);
                        }

                        il.Emit(OpCodes.Stloc, localReturnValue);
                        il.Emit(OpCodes.Ldloc, localReturnValue);
                    }

                    il.Emit(OpCodes.Ret);
                }

                return typeBuilder.CreateType();
            });
            return type;
        }

        public static Type CreateClassProxy(Type classType, params Type[] interfaceTypes)
        {
            if (classType.IsSealed)
                throw new InvalidOperationException("the implementType is sealed");

            //
            var proxyTypeName = $"{ProxyAssemblyName}.{classType.FullName}.{classType.FullName}";

            var type = _proxyTypes.GetOrAdd(proxyTypeName, name =>
            {
                var typeBuilder = _moduleBuilder.DefineType(proxyTypeName, TypeAttributes.Public, classType, interfaceTypes);

                foreach (var constructor in classType.GetConstructors())
                {
                    var constructorTypes = constructor.GetParameters().Select(o => o.ParameterType).ToArray();
                    var constructorBuilder = typeBuilder.DefineConstructor(
                        MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.RTSpecialName | MethodAttributes.SpecialName,
                        CallingConventions.Standard,
                        constructorTypes);
                    foreach (var customAttribute in constructor.CustomAttributes)
                    {
                        constructorBuilder.SetCustomAttribute(DefineCustomAttribute(customAttribute));
                    }
                    var il = constructorBuilder.GetILGenerator();
                    il.Emit(OpCodes.Ldarg_0);

                    for (var i = 0; i < constructorTypes.Length; i++)
                    {
                        il.Emit(OpCodes.Ldarg, i + 1);
                    }

                    il.Call(constructor);
                    il.Emit(OpCodes.Nop);
                    il.Emit(OpCodes.Ret);
                }

                var methods = interfaceTypes.IsNullOrEmpty()
                    ? classType.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    : interfaceTypes.Select(it => it.GetMethods()).SelectMany(m => m).ToArray();
                foreach (var method in methods)
                {
                    if (method.IsFinal || _ignoredMethodNames.Contains(method.Name))
                    {
                        continue;
                    }
                    var methodParameterTypes = method.GetParameters()
                        .Select(p => p.ParameterType)
                        .ToArray();
                    var methodBuilder = typeBuilder.DefineMethod(method.Name,
                        MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig |
                          MethodAttributes.Virtual | MethodAttributes.NewSlot,
                        method.CallingConvention,
                        method.ReturnType,
                        methodParameterTypes
                        );
                    typeBuilder.DefineMethodOverride(methodBuilder, method);

                    var il = methodBuilder.GetILGenerator();

                    var localReturnValue = il.DeclareReturnValue(method.ReturnType);
                    var localCurrentMethod = il.DeclareLocal(typeof(MethodInfo));
                    var localMethodBase = il.DeclareLocal(typeof(MethodInfo));
                    var localParameters = il.DeclareLocal(typeof(object[]));
                    var localTarget = il.DeclareLocal(typeof(object));

                    // var currentMethod = MethodBase.GetCurrentMethod();
                    il.Call(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod)));
                    il.EmitCastToType(typeof(MethodBase), typeof(MethodInfo));
                    il.Emit(OpCodes.Stloc, localCurrentMethod);

                    il.Emit(OpCodes.Ldloc, localCurrentMethod);
                    il.Call(typeof(AspectExtensions).GetMethod(nameof(AspectExtensions.GetBaseMethod)));
                    il.Emit(OpCodes.Stloc, localMethodBase);

                    // var parameters = new[] {a, b, c};
                    il.Emit(OpCodes.Ldc_I4, methodParameterTypes.Length);
                    il.Emit(OpCodes.Newarr, typeof(object));
                    if (methodParameterTypes.Length > 0)
                    {
                        for (var i = 0; i < methodParameterTypes.Length; i++)
                        {
                            il.Emit(OpCodes.Dup);
                            il.Emit(OpCodes.Ldc_I4, i);
                            il.Emit(OpCodes.Ldarg, i + 1);
                            if (methodParameterTypes[i].IsValueType)
                            {
                                il.Emit(OpCodes.Box, methodParameterTypes[i].UnderlyingSystemType);
                            }

                            il.Emit(OpCodes.Stelem_Ref);
                        }
                    }
                    il.Emit(OpCodes.Stloc, localParameters);

                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Stloc, localTarget);

                    // var invocation = new MethodInvocation(method, methodBase, this, parameters);
                    var localAspectInvocation = il.DeclareLocal(typeof(MethodInvocation));
                    il.Emit(OpCodes.Ldloc, localCurrentMethod);
                    il.Emit(OpCodes.Ldloc, localMethodBase);
                    il.Emit(OpCodes.Ldarg_0);
                    il.Emit(OpCodes.Ldloc, localTarget);
                    il.Emit(OpCodes.Ldloc, localParameters);

                    il.New(typeof(MethodInvocation).GetConstructors()[0]);
                    il.Emit(OpCodes.Stloc, localAspectInvocation);

                    // AspectDelegate.InvokeAspectDelegate(invocation);
                    il.Emit(OpCodes.Ldloc, localAspectInvocation);
                    var invokeAspectDelegateMethod =
                        typeof(AspectDelegate).GetMethod(nameof(AspectDelegate.Invoke), new[] { typeof(IInvocation) });
                    il.Call(invokeAspectDelegateMethod);
                    il.Emit(OpCodes.Nop);

                    if (method.ReturnType != typeof(void))
                    {
                        // load return value
                        il.Emit(OpCodes.Ldloc, localAspectInvocation);
                        var getMethod = typeof(MethodInvocation).GetProperty("ReturnValue").GetGetMethod();
                        il.EmitCall(OpCodes.Callvirt, getMethod, Type.EmptyTypes);

                        if (method.ReturnType.IsValueType)
                        {
                            il.EmitCastToType(typeof(object), method.ReturnType);
                        }

                        il.Emit(OpCodes.Stloc, localReturnValue);
                        il.Emit(OpCodes.Ldloc, localReturnValue);
                    }

                    il.Emit(OpCodes.Ret);
                }

                return typeBuilder.CreateType();
            });
            return type;
        }

        private static CustomAttributeBuilder DefineCustomAttribute(CustomAttributeData customAttributeData)
        {
            if (customAttributeData.NamedArguments != null)
            {
                var attributeTypeInfo = customAttributeData.AttributeType.GetTypeInfo();
                //var constructorArgs = customAttributeData.ConstructorArguments.Select(c => c.Value).ToArray();
                var constructorArgs = customAttributeData.ConstructorArguments
                    .Select(ReadAttributeValue)
                    .ToArray();
                var namedProperties = customAttributeData.NamedArguments
                        .Where(n => !n.IsField)
                        .Select(n => attributeTypeInfo.GetProperty(n.MemberName))
                        .ToArray();
                var propertyValues = customAttributeData.NamedArguments
                         .Where(n => !n.IsField)
                         .Select(n => ReadAttributeValue(n.TypedValue))
                         .ToArray();
                var namedFields = customAttributeData.NamedArguments.Where(n => n.IsField)
                         .Select(n => attributeTypeInfo.GetField(n.MemberName))
                         .ToArray();
                var fieldValues = customAttributeData.NamedArguments.Where(n => n.IsField)
                         .Select(n => ReadAttributeValue(n.TypedValue))
                         .ToArray();
                return new CustomAttributeBuilder(customAttributeData.Constructor, constructorArgs
                   , namedProperties
                   , propertyValues, namedFields, fieldValues);
            }

            return new CustomAttributeBuilder(customAttributeData.Constructor,
                customAttributeData.ConstructorArguments.Select(c => c.Value).ToArray());
        }

        private static object ReadAttributeValue(CustomAttributeTypedArgument argument)
        {
            var value = argument.Value;
            if (argument.ArgumentType.GetTypeInfo().IsArray == false)
            {
                return value;
            }
            //special case for handling arrays in attributes
            //the actual type of "value" is ReadOnlyCollection<CustomAttributeTypedArgument>.
            var arguments = ((IEnumerable<CustomAttributeTypedArgument>)value)
                .Select(m => m.Value)
                .ToArray();
            return arguments;
        }
    }
}
