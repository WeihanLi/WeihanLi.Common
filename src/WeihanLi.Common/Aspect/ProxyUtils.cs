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
        private const string ProxyAssemblyName = "WeihanLi.Aspects.DynamicGenerated";

        private const MethodAttributes OverrideMethodAttributes = MethodAttributes.HideBySig | MethodAttributes.Virtual;
        private const MethodAttributes InterfaceMethodAttributes = MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;

        private static readonly ModuleBuilder _moduleBuilder;
        private static readonly ConcurrentDictionary<string, Type> _proxyTypes = new ConcurrentDictionary<string, Type>();

        private const string TargetFieldName = "__target";

        private static readonly Func<Type, Type, string> _proxyTypeNameResolver;

        static ProxyUtils()
        {
            var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(ProxyAssemblyName), AssemblyBuilderAccess.Run);
            _moduleBuilder = asmBuilder.DefineDynamicModule("Default");

            _proxyTypeNameResolver = (serviceType, implementType) =>
            {
                var typeName1 = serviceType.GetFriendlyTypeName();
                var typeName2 = implementType.GetFriendlyTypeName();

                return $"{ProxyAssemblyName}.{typeName1}.{typeName2}".TrimEnd('.');
            };
        }

        private static string GetFriendlyTypeName(this Type type)
        {
            if (null == type)
                return string.Empty;
            if (type.IsGenericType)
            {
                var genericArgs = type.GetGenericArguments();
                var genericArgsName = genericArgs.Select(t => t.GetFriendlyTypeName())
                    .StringJoin("_");
                return $"{type.FullName}__{genericArgsName}";
            }

            return type.FullName;
        }

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
            var proxyTypeName = _proxyTypeNameResolver(interfaceType, null);
            var type = _proxyTypes.GetOrAdd(proxyTypeName, name =>
            {
                var typeBuilder = _moduleBuilder.DefineType(proxyTypeName, TypeAttributes.Public | TypeAttributes.Class | TypeAttributes.Sealed, typeof(object), new[] { interfaceType });

                GenericParameterUtils.DefineGenericParameter(interfaceType, typeBuilder);

                // define default constructor
                typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

                // properties
                var propertyMethods = new HashSet<string>();
                var properties = interfaceType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var property in properties)
                {
                    var propertyBuilder = typeBuilder.DefineProperty(property.Name, property.Attributes, property.PropertyType, Type.EmptyTypes);
                    var field = typeBuilder.DefineField($"_{property.Name}", property.PropertyType, FieldAttributes.Private);
                    if (property.CanRead)
                    {
                        var methodBuilder = typeBuilder.DefineMethod(property.GetMethod.Name, InterfaceMethodAttributes, property.GetMethod.CallingConvention, property.GetMethod.ReturnType, property.GetMethod.GetParameters().Select(p => p.ParameterType).ToArray());
                        var ilGen = methodBuilder.GetILGenerator();
                        ilGen.Emit(OpCodes.Ldarg_0);
                        ilGen.Emit(OpCodes.Ldfld, field);
                        ilGen.Emit(OpCodes.Ret);
                        typeBuilder.DefineMethodOverride(methodBuilder, property.GetMethod);
                        propertyBuilder.SetGetMethod(methodBuilder);
                        propertyMethods.Add(property.GetMethod.Name);
                    }
                    if (property.CanWrite)
                    {
                        var methodBuilder = typeBuilder.DefineMethod(property.SetMethod.Name, InterfaceMethodAttributes, property.SetMethod.CallingConvention, property.SetMethod.ReturnType, property.SetMethod.GetParameters().Select(p => p.ParameterType).ToArray());
                        var ilGen = methodBuilder.GetILGenerator();
                        ilGen.Emit(OpCodes.Ldarg_0);
                        ilGen.Emit(OpCodes.Ldarg_1);
                        ilGen.Emit(OpCodes.Stfld, field);
                        ilGen.Emit(OpCodes.Ret);
                        typeBuilder.DefineMethodOverride(methodBuilder, property.SetMethod);
                        propertyBuilder.SetSetMethod(methodBuilder);
                        propertyMethods.Add(property.SetMethod.Name);
                    }
                    foreach (var customAttributeData in property.CustomAttributes)
                    {
                        propertyBuilder.SetCustomAttribute(DefineCustomAttribute(customAttributeData));
                    }
                }

                // methods
                var methods = interfaceType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                foreach (var method in methods.Where(x => !propertyMethods.Contains(x.Name)))
                {
                    MethodUtils.DefineInterfaceMethod(typeBuilder, method, null);
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
            var proxyTypeName = _proxyTypeNameResolver(interfaceType, implementType);

            var type = _proxyTypes.GetOrAdd(proxyTypeName, name =>
            {
                var typeBuilder = _moduleBuilder.DefineType(proxyTypeName, implementType.Attributes, implementType, new[] { interfaceType });
                GenericParameterUtils.DefineGenericParameter(interfaceType, typeBuilder);

                var targetField = typeBuilder.DefineField(TargetFieldName, implementType, FieldAttributes.Private);
                // constructors
                foreach (var constructor in implementType.GetConstructors())
                {
                    var constructorTypes = constructor.GetParameters().Select(o => o.ParameterType).ToArray();
                    var constructorBuilder = typeBuilder.DefineConstructor(
                        constructor.Attributes,
                        constructor.CallingConvention,
                        constructorTypes);
                    foreach (var customAttribute in constructor.CustomAttributes)
                    {
                        constructorBuilder.SetCustomAttribute(DefineCustomAttribute(customAttribute));
                    }
                    var il = constructorBuilder.GetILGenerator();

                    il.EmitThis();
                    il.EmitThis();
                    il.Emit(OpCodes.Stfld, targetField);

                    il.Emit(OpCodes.Ldarg_0);

                    for (var i = 0; i < constructorTypes.Length; i++)
                    {
                        il.Emit(OpCodes.Ldarg, i + 1);
                    }

                    il.Call(constructor);
                    il.Emit(OpCodes.Nop);
                    il.Emit(OpCodes.Ret);
                }

                // properties
                var propertyMethods = new HashSet<string>();
                var properties = interfaceType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                foreach (var property in properties)
                {
                    var propertyBuilder = typeBuilder.DefineProperty(property.Name, property.Attributes, property.PropertyType, Type.EmptyTypes);
                    var field = typeBuilder.DefineField($"_{property.Name}", property.PropertyType, FieldAttributes.Private);
                    if (property.CanRead)
                    {
                        var methodBuilder = MethodUtils.DefineInterfaceMethod(typeBuilder, property.GetMethod, targetField);
                        typeBuilder.DefineMethodOverride(methodBuilder, property.GetMethod);
                        propertyBuilder.SetGetMethod(methodBuilder);
                        propertyMethods.Add(property.GetMethod.Name);
                    }
                    if (property.CanWrite)
                    {
                        var methodBuilder = MethodUtils.DefineInterfaceMethod(typeBuilder, property.SetMethod, targetField);
                        typeBuilder.DefineMethodOverride(methodBuilder, property.SetMethod);
                        propertyBuilder.SetSetMethod(methodBuilder);
                        propertyMethods.Add(property.SetMethod.Name);
                    }
                    foreach (var customAttributeData in property.CustomAttributes)
                    {
                        propertyBuilder.SetCustomAttribute(DefineCustomAttribute(customAttributeData));
                    }
                }

                //
                var methods = interfaceType.GetMethods(BindingFlags.Instance | BindingFlags.Public);
                foreach (var method in methods)
                {
                    if (propertyMethods.Contains(method.Name))
                    {
                        continue;
                    }
                    MethodUtils.DefineInterfaceMethod(typeBuilder, method, targetField);
                }

                return typeBuilder.CreateType();
            });
            return type;
        }

        public static Type CreateClassProxy(Type classType, Type implementType)
        {
            if (classType.IsSealed)
            {
                throw new InvalidOperationException("the class type is sealed");
            }
            //
            var proxyTypeName = _proxyTypeNameResolver(classType, implementType);
            var type = _proxyTypes.GetOrAdd(proxyTypeName, name =>
            {
                var typeBuilder = _moduleBuilder.DefineType(proxyTypeName, TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.Class, implementType, Type.EmptyTypes);
                GenericParameterUtils.DefineGenericParameter(classType, typeBuilder);

                var targetField = typeBuilder.DefineField(TargetFieldName, implementType, FieldAttributes.Private);

                // constructors
                foreach (var constructor in classType.GetConstructors())
                {
                    var constructorTypes = constructor.GetParameters().Select(o => o.ParameterType).ToArray();
                    var constructorBuilder = typeBuilder.DefineConstructor(
                        constructor.Attributes,
                        constructor.CallingConvention,
                        constructorTypes);
                    foreach (var customAttribute in constructor.CustomAttributes)
                    {
                        constructorBuilder.SetCustomAttribute(DefineCustomAttribute(customAttribute));
                    }
                    var il = constructorBuilder.GetILGenerator();

                    il.EmitNull();
                    il.EmitThis();
                    il.Emit(OpCodes.Stfld, targetField);

                    il.Emit(OpCodes.Ldarg_0);
                    for (var i = 0; i < constructorTypes.Length; i++)
                    {
                        il.Emit(OpCodes.Ldarg, i + 1);
                    }
                    il.Call(constructor);

                    il.Emit(OpCodes.Nop);

                    il.Emit(OpCodes.Ret);
                }

                // properties
                var propertyMethods = new HashSet<string>();
                foreach (var property in classType.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
                {
                    if (property.IsVisibleAndVirtual())
                    {
                        var propertyBuilder = typeBuilder.DefineProperty(name, property.Attributes, property.PropertyType, Type.EmptyTypes);

                        //inherit targetMethod's attribute
                        foreach (var customAttributeData in property.CustomAttributes)
                        {
                            propertyBuilder.SetCustomAttribute(DefineCustomAttribute(customAttributeData));
                        }

                        if (property.CanRead)
                        {
                            propertyMethods.Add(property.GetMethod.Name);

                            var method = MethodUtils.DefineClassMethod(typeBuilder, property.GetMethod, targetField);
                            propertyBuilder.SetGetMethod(method);
                        }
                        if (property.CanWrite)
                        {
                            propertyMethods.Add(property.SetMethod.Name);

                            var method = MethodUtils.DefineClassMethod(typeBuilder, property.SetMethod, targetField);
                            propertyBuilder.SetSetMethod(method);
                        }
                    }
                }

                // methods
                var methods = classType.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                        .Where(m => m.IsVirtual && m.IsVisible() && !propertyMethods.Contains(m.Name))
                        .ToArray();
                foreach (var method in methods)
                {
                    MethodUtils.DefineClassMethod(typeBuilder, method, targetField);
                }

                return typeBuilder.CreateType();
            });
            return type;
        }

        public static void SetProxyTarget(object proxyService, object target)
        {
            if (null != proxyService && null != target)
            {
                var targetField = proxyService.GetField(TargetFieldName, BindingFlags.Instance | BindingFlags.NonPublic);
                if (targetField != null && targetField.GetType().IsInstanceOfType(target))
                {
                    targetField.SetValue(proxyService, target);
                }
            }
        }

        private static class MethodUtils
        {
            public static MethodBuilder DefineInterfaceMethod(TypeBuilder typeBuilder, MethodInfo method, FieldBuilder targetField)
            {
                var methodParameterTypes = method.GetParameters()
                        .Select(p => p.ParameterType)
                        .ToArray();
                var methodBuilder = typeBuilder.DefineMethod(method.Name
                    , InterfaceMethodAttributes,
                    method.CallingConvention,
                    method.ReturnType,
                    methodParameterTypes
                    );

                GenericParameterUtils.DefineGenericParameter(method, methodBuilder);

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

                // var currentMethod = MethodBase.GetCurrentMethod();
                il.Call(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod)));
                il.EmitConvertToType(typeof(MethodBase), typeof(MethodInfo));
                il.Emit(OpCodes.Stloc, localCurrentMethod);

                if (method.IsGenericMethodDefinition)
                {
                    //il.EmitMethod(method.MakeGenericMethod(methodBuilder.GetGenericArguments()));
                    //il.EmitMethod(methodBuilder.MakeGenericMethod(methodBuilder.GetGenericArguments()));
                }

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

                // var aspectInvocation = new AspectInvocation(method, this, parameters);
                var localAspectInvocation = il.DeclareLocal(typeof(AspectInvocation));
                il.Emit(OpCodes.Ldloc, localCurrentMethod);
                il.Emit(OpCodes.Ldloc, localMethodBase);
                il.EmitThis();

                il.EmitThis();
                if (null != targetField)
                {
                    il.Emit(OpCodes.Ldfld, targetField);
                }

                il.Emit(OpCodes.Ldloc, localParameters);

                il.New(typeof(AspectInvocation).GetConstructors()[0]);
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
                    il.EmitCall(OpCodes.Callvirt, InternalAspectHelper.GetInvocationReturnValueMethod, Type.EmptyTypes);

                    if (method.ReturnType != typeof(object))
                    {
                        il.EmitCastToType(typeof(object), method.ReturnType);
                    }

                    il.Emit(OpCodes.Stloc, localReturnValue);
                    il.Emit(OpCodes.Ldloc, localReturnValue);
                }

                il.Emit(OpCodes.Ret);
                return methodBuilder;
            }

            public static MethodBuilder DefineClassMethod(TypeBuilder typeBuilder, MethodInfo method, FieldBuilder targetField)
            {
                var methodParameterTypes = method.GetParameters()
                    .Select(p => p.ParameterType)
                    .ToArray();

                var methodAttributes = OverrideMethodAttributes;
                if (method.Attributes.HasFlag(MethodAttributes.Public))
                {
                    methodAttributes |= MethodAttributes.Public;
                }
                if (method.Attributes.HasFlag(MethodAttributes.Family))
                {
                    methodAttributes |= MethodAttributes.Family;
                }
                if (method.Attributes.HasFlag(MethodAttributes.FamORAssem))
                {
                    methodAttributes |= MethodAttributes.FamORAssem;
                }
                var methodBuilder = typeBuilder.DefineMethod(method.Name,
                    methodAttributes,
                    method.CallingConvention,
                    method.ReturnType,
                    methodParameterTypes
                );

                GenericParameterUtils.DefineGenericParameter(method, methodBuilder);

                foreach (var customAttributeData in method.CustomAttributes)
                {
                    methodBuilder.SetCustomAttribute(DefineCustomAttribute(customAttributeData));
                }

                var il = methodBuilder.GetILGenerator();

                var localReturnValue = il.DeclareReturnValue(method.ReturnType);
                var localCurrentMethod = il.DeclareLocal(typeof(MethodInfo));
                var localMethodBase = il.DeclareLocal(typeof(MethodInfo));
                var localParameters = il.DeclareLocal(typeof(object[]));

                // var currentMethod = MethodBase.GetCurrentMethod();
                il.Call(typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod)));
                il.EmitConvertToType(typeof(MethodBase), typeof(MethodInfo));
                il.Emit(OpCodes.Stloc, localCurrentMethod);

                if (method.IsGenericMethodDefinition)
                {
                    //il.EmitMethod(method.MakeGenericMethod(methodBuilder.GetGenericArguments()));
                    //il.EmitMethod(methodBuilder.MakeGenericMethod(methodBuilder.GetGenericArguments()));
                }

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

                // var invocation = new MethodInvocation(method, methodBase, this, this, parameters);
                var localAspectInvocation = il.DeclareLocal(typeof(AspectInvocation));
                il.Emit(OpCodes.Ldloc, localCurrentMethod);
                il.Emit(OpCodes.Ldloc, localMethodBase);
                il.EmitThis();
                il.EmitThis();
                il.Emit(OpCodes.Ldfld, targetField);
                il.Emit(OpCodes.Ldloc, localParameters);

                il.New(typeof(AspectInvocation).GetConstructors()[0]);
                il.Emit(OpCodes.Stloc, localAspectInvocation);

                // AspectDelegate.InvokeAspectDelegate(invocation);
                il.Emit(OpCodes.Ldloc, localAspectInvocation);
                var invokeAspectDelegateMethod =
                    typeof(AspectDelegate).GetMethod(nameof(AspectDelegate.Invoke), new[] { typeof(IInvocation) });
                il.Call(invokeAspectDelegateMethod);

                if (method.ReturnType != typeof(void))
                {
                    // load return value
                    il.Emit(OpCodes.Ldloc, localAspectInvocation);

                    il.EmitCall(OpCodes.Callvirt, InternalAspectHelper.GetInvocationReturnValueMethod, Type.EmptyTypes);

                    if (method.ReturnType.IsValueType)
                    {
                        il.EmitCastToType(typeof(object), method.ReturnType);
                    }

                    il.Emit(OpCodes.Stloc, localReturnValue);
                    il.Emit(OpCodes.Ldloc, localReturnValue);
                }

                il.Emit(OpCodes.Ret);

                return methodBuilder;
            }
        }

        private static class GenericParameterUtils
        {
            public static void DefineGenericParameter(Type targetType, TypeBuilder typeBuilder)
            {
                if (!targetType.IsGenericTypeDefinition)
                {
                    return;
                }
                var genericArguments = targetType.GetGenericArguments();
                var genericArgumentsBuilders = typeBuilder
                    .DefineGenericParameters(genericArguments.Select(a => a.Name).ToArray());
                for (var index = 0; index < genericArguments.Length; index++)
                {
                    genericArgumentsBuilders[index].SetGenericParameterAttributes(ToClassGenericParameterAttributes(genericArguments[index].GenericParameterAttributes));
                    foreach (var constraint in genericArguments[index].GetGenericParameterConstraints())
                    {
                        if (constraint.IsClass)
                        {
                            genericArgumentsBuilders[index].SetBaseTypeConstraint(constraint);
                        }
                        if (constraint.IsInterface)
                        {
                            genericArgumentsBuilders[index].SetInterfaceConstraints(constraint);
                        }
                    }
                }
            }

            public static void DefineGenericParameter(MethodInfo targetMethod, MethodBuilder methodBuilder)
            {
                if (!targetMethod.IsGenericMethod)
                {
                    return;
                }
                var genericArguments = targetMethod.GetGenericArguments();
                var genericArgumentsBuilders = methodBuilder
                    .DefineGenericParameters(genericArguments.Select(a => a.Name).ToArray());
                for (var index = 0; index < genericArguments.Length; index++)
                {
                    genericArgumentsBuilders[index].SetGenericParameterAttributes(genericArguments[index].GenericParameterAttributes);
                    foreach (var constraint in genericArguments[index].GetGenericParameterConstraints())
                    {
                        if (constraint.IsClass)
                        {
                            genericArgumentsBuilders[index].SetBaseTypeConstraint(constraint);
                        }
                        if (constraint.IsInterface)
                        {
                            genericArgumentsBuilders[index].SetInterfaceConstraints(constraint);
                        }
                    }
                }
            }

            private static GenericParameterAttributes ToClassGenericParameterAttributes(GenericParameterAttributes attributes)
            {
                if (attributes == GenericParameterAttributes.None)
                {
                    return GenericParameterAttributes.None;
                }
                if (attributes.HasFlag(GenericParameterAttributes.SpecialConstraintMask))
                {
                    return GenericParameterAttributes.SpecialConstraintMask;
                }
                if (attributes.HasFlag(GenericParameterAttributes.NotNullableValueTypeConstraint))
                {
                    return GenericParameterAttributes.NotNullableValueTypeConstraint;
                }
                if (attributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint) && attributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
                {
                    return GenericParameterAttributes.ReferenceTypeConstraint | GenericParameterAttributes.DefaultConstructorConstraint;
                }
                if (attributes.HasFlag(GenericParameterAttributes.ReferenceTypeConstraint))
                {
                    return GenericParameterAttributes.ReferenceTypeConstraint;
                }
                if (attributes.HasFlag(GenericParameterAttributes.DefaultConstructorConstraint))
                {
                    return GenericParameterAttributes.DefaultConstructorConstraint;
                }
                return GenericParameterAttributes.None;
            }
        }

        private static void DefineParameters(this MethodInfo targetMethod, MethodBuilder methodBuilder)
        {
            var parameters = targetMethod.GetParameters();
            if (parameters.Length > 0)
            {
                var paramOffset = 1;   // 1
                for (var i = 0; i < parameters.Length; i++)
                {
                    var parameter = parameters[i];
                    var parameterBuilder = methodBuilder.DefineParameter(i + paramOffset, parameter.Attributes, parameter.Name);
                    if (parameter.HasDefaultValue)
                    {
                        if (!(parameter.ParameterType.GetTypeInfo().IsValueType && parameter.DefaultValue == null))
                            parameterBuilder.SetConstant(parameter.DefaultValue);
                    }

                    foreach (var attribute in parameter.CustomAttributes)
                    {
                        parameterBuilder.SetCustomAttribute(DefineCustomAttribute(attribute));
                    }
                }
            }

            var returnParameter = targetMethod.ReturnParameter;
            if (null != returnParameter)
            {
                var returnParameterBuilder = methodBuilder.DefineParameter(0, returnParameter.Attributes, returnParameter.Name);
                foreach (var attribute in returnParameter.CustomAttributes)
                {
                    returnParameterBuilder.SetCustomAttribute(DefineCustomAttribute(attribute));
                }
            }
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

#if NETSTANDARD2_0

        private static Type CreateType(this TypeBuilder typeBuilder)
        {
            return typeBuilder.CreateTypeInfo()?.AsType();
        }

#endif
    }
}
