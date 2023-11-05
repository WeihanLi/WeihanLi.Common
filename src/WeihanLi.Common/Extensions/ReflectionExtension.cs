﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using WeihanLi.Common;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class ReflectionExtension
{
    public static MethodInfo? GetMethodBySignature(this Type type, MethodInfo method)
    {
        if (type == null)
        {
            throw new ArgumentNullException(nameof(type));
        }
        if (method == null)
        {
            throw new ArgumentNullException(nameof(method));
        }
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(x => x.Name.Equals(method.Name))
            .ToArray();

        var parameterTypes = method.GetParameters().Select(x => x.ParameterType).ToArray();
        if (method.ContainsGenericParameters)
        {
            foreach (var info in methods)
            {
                var innerParams = info.GetParameters();
                if (innerParams.Length != parameterTypes.Length)
                {
                    continue;
                }

                var idx = 0;
                foreach (var param in innerParams)
                {
                    if (!param.ParameterType.IsGenericParameter
                        && !parameterTypes[idx].IsGenericParameter
                        && param.ParameterType != parameterTypes[idx]
                    )
                    {
                        break;
                    }

                    idx++;
                }
                if (idx < parameterTypes.Length)
                {
                    continue;
                }

                return info;
            }

            return null;
        }

        var baseMethod = type.GetMethod(method.Name, parameterTypes);
        return baseMethod;
    }

    public static MethodInfo? GetBaseMethod(this MethodInfo? currentMethod)
    {
        if (null == currentMethod?.DeclaringType?.BaseType)
            return null;

        return currentMethod.DeclaringType.BaseType.GetMethodBySignature(currentMethod);
    }

    public static bool IsVisibleAndVirtual(this PropertyInfo property)
    {
        if (property == null)
        {
            throw new ArgumentNullException(nameof(property));
        }
        return (property.CanRead && property.GetMethod!.IsVisibleAndVirtual()) ||
               (property.CanWrite && property.SetMethod!.IsVisibleAndVirtual());
    }

    public static bool IsVisibleAndVirtual(this MethodInfo method)
    {
        if (method == null)
        {
            throw new ArgumentNullException(nameof(method));
        }
        if (method.IsStatic || method.IsFinal)
        {
            return false;
        }
        return method.IsVirtual &&
               (method.IsPublic || method.IsFamily || method.IsFamilyOrAssembly);
    }

    public static bool IsVisible(this MethodBase method)
    {
        if (method == null)
        {
            throw new ArgumentNullException(nameof(method));
        }
        return method.IsPublic || method.IsFamily || method.IsFamilyOrAssembly;
    }

    /// <summary>
    /// An object extension method that gets DisplayName if DisplayAttribute does not exist,return the MemberName
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>The custom attribute.</returns>
    public static string GetDisplayName(this MemberInfo @this)
        => @this.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? @this.GetCustomAttribute<DisplayAttribute>()?.Name ?? @this.Name;

    /// <summary>
    /// GetColumnName
    /// </summary>
    /// <returns></returns>
    public static string GetColumnName(this PropertyInfo propertyInfo) => propertyInfo.GetCustomAttribute<ColumnAttribute>()?.Name ?? propertyInfo.Name;

    /// <summary>
    /// GetDescription
    /// </summary>
    /// <returns></returns>
    public static string GetDescription(this MemberInfo @this) => @this.GetCustomAttribute<DescriptionAttribute>()?.Description ?? @this.Name;

    /// <summary>A T extension method that searches for the public field with the specified name.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="name">The string containing the name of the data field to get.</param>
    /// <returns>
    ///     An object representing the field that matches the specified requirements, if found;
    ///     otherwise, null.
    /// </returns>
    public static FieldInfo? GetField<T>(this T @this, string name)
    {
        return CacheUtil.GetTypeFields(typeof(T)).FirstOrDefault(_ => _.Name == name);
    }

    /// <summary>
    ///     A T extension method that searches for the specified field, using the specified
    ///     binding constraints.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="name">The string containing the name of the data field to get.</param>
    /// <param name="bindingAttr">
    ///     A bitmask comprised of one or more BindingFlags that specify how the
    ///     search is conducted.
    /// </param>
    /// <returns>
    ///     An object representing the field that matches the specified requirements, if found;
    ///     otherwise, null.
    /// </returns>
    public static FieldInfo? GetField<T>(this T @this, string name, BindingFlags bindingAttr)
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        return @this.GetType().GetField(name, bindingAttr);
    }

    /// <summary>An object extension method that gets the fields.</summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>An array of field information.</returns>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields)]
    public static FieldInfo[] GetFields(this object @this)
    {
        return CacheUtil.GetTypeFields(Guard.NotNull(@this, nameof(@this)).GetType());
    }

    /// <summary>An object extension method that gets the fields.</summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="bindingAttr">The binding attribute.</param>
    /// <returns>An array of field information.</returns>
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicFields | DynamicallyAccessedMemberTypes.NonPublicFields)]
    public static FieldInfo[] GetFields(this object @this, BindingFlags bindingAttr)
    {
        return @this.GetType().GetFields(bindingAttr);
    }

    /// <summary>
    ///     A T extension method that gets a field value (Public | NonPublic | Instance | Static)
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <returns>The field value.</returns>
    public static object? GetFieldValue<T>(this T @this, string fieldName)
    {
        var field = @this.GetField(fieldName);
        return field?.GetValue(@this);
    }

    /// <summary>
    ///     A T extension method that searches for the public method with the specified name.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="name">The string containing the name of the public method to get.</param>
    /// <returns>
    ///     An object that represents the public method with the specified name, if found; otherwise, null.
    /// </returns>
    public static MethodInfo GetMethod<T>(this T @this, string name)
    {
        return CacheUtil.TypeMethodCache.GetOrAdd(typeof(T), t => t.GetMethods()).First(_ => _.Name == name);
    }

    /// <summary>
    ///     A T extension method that searches for the specified method whose parameters match the specified argument
    ///     types and modifiers, using the specified binding constraints.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="name">The string containing the name of the public method to get.</param>
    /// <param name="bindingAttr">A bitmask comprised of one or more BindingFlags that specify how the search is conducted.</param>
    /// <returns>
    ///     An object that represents the public method with the specified name, if found; otherwise, null.
    /// </returns>
    public static MethodInfo? GetMethod<T>(this T @this, string name, BindingFlags bindingAttr)
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        return @this.GetType().GetMethod(name, bindingAttr);
    }

    /// <summary>
    ///     A T extension method that returns all the public methods of the current Type.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <returns>
    ///     An array of MethodInfo objects representing all the public methods defined for the current Type. or An empty
    ///     array of type MethodInfo, if no public methods are defined for the current Type.
    /// </returns>
    public static MethodInfo[] GetMethods<T>(this T @this)
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        return CacheUtil.TypeMethodCache.GetOrAdd(@this.GetType(), t => t.GetMethods());
    }

    /// <summary>
    ///     A T extension method that searches for the methods defined for the current Type, using the specified binding
    ///     constraints.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="bindingAttr">A bitmask comprised of one or more BindingFlags that specify how the search is conducted.</param>
    /// <returns>
    ///     An array of MethodInfo objects representing all methods defined for the current Type that match the specified
    ///     binding constraints. or An empty array of type MethodInfo, if no methods are defined for the current Type, or
    ///     if none of the defined methods match the binding constraints.
    /// </returns>
    public static MethodInfo[] GetMethods<T>(this T @this, BindingFlags bindingAttr)
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        return @this.GetType().GetMethods(bindingAttr);
    }

    /// <summary>An object extension method that gets the properties.</summary>
    /// <param name="this">The @this to act on.</param>
    /// <returns>An array of property information.</returns>
    public static PropertyInfo[] GetProperties(this object @this)
    {
        return CacheUtil.GetTypeProperties(@this.GetType());
    }

    /// <summary>An object extension method that gets the properties.</summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="bindingAttr">The binding attribute.</param>
    /// <returns>An array of property information.</returns>
    public static PropertyInfo[] GetProperties(this object @this, BindingFlags bindingAttr)
    {
        return @this.GetType().GetProperties(bindingAttr);
    }

    /// <summary>
    ///     A T extension method that gets a property.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="name">The name.</param>
    /// <returns>The property.</returns>
    public static PropertyInfo? GetProperty<T>(this T @this, string name)
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        return CacheUtil.GetTypeProperties(@this.GetType()).FirstOrDefault(_ => _.Name == name);
    }

    /// <summary>
    ///     A T extension method that gets a property.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="name">The name.</param>
    /// <param name="bindingAttr">The binding attribute.</param>
    /// <returns>The property.</returns>
    public static PropertyInfo? GetProperty<T>(this T @this, string name, BindingFlags bindingAttr)
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        return @this.GetType().GetProperty(name, bindingAttr);
    }

    /// <summary>A T extension method that gets property or field.</summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="name">The name.</param>
    /// <returns>The property or field.</returns>
    public static MemberInfo? GetPropertyOrField<T>(this T @this, string name)
    {
        var property = @this.GetProperty(name);
        if (property != null)
        {
            return property;
        }

        var field = @this.GetField(name);
        return field;
    }

    /// <summary>
    ///     A T extension method that gets property value.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <returns>The property value.</returns>
    public static object? GetPropertyValue<T>(this T @this, string propertyName)
    {
        var property = @this.GetProperty(propertyName);
        return property?.GetValueGetter<T>()?.Invoke(@this);
    }

    /// <summary>
    ///     An object extension method that query if '@this' is attribute defined.
    /// </summary>
    /// <param name="this">The @this to act on.</param>
    /// <param name="attributeType">Type of the attribute.</param>
    /// <param name="inherit">true to inherit.</param>
    /// <returns>true if attribute defined, false if not.</returns>
    public static bool IsAttributeDefined(this object @this, Type attributeType, bool inherit = true)
    {
        return @this.GetType().IsDefined(attributeType, inherit);
    }

    /// <summary>
    ///     An object extension method that query if '@this' is attribute defined.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="inherit">true to inherit.</param>
    /// <returns>true if attribute defined, false if not.</returns>
    public static bool IsAttributeDefined<T>(this object @this, bool inherit = true) where T : Attribute
    {
        return @this.GetType().IsDefined(typeof(T), inherit);
    }

    /// <summary>
    ///     A T extension method that sets field value.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="fieldName">Name of the field.</param>
    /// <param name="value">The value.</param>
    public static void SetFieldValue<T>(this T @this, string fieldName, object? value)
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        var type = @this.GetType();
        var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        field?.SetValue(@this, value);
    }

    /// <summary>
    ///     A T extension method that sets property value.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="propertyName">Name of the property.</param>
    /// <param name="value">The value.</param>
    public static void SetPropertyValue<T>(this T @this, string propertyName, object? value) where T : class
    {
        var property = @this.GetProperty(propertyName);
        property?.GetValueSetter()?.Invoke(@this, value);
    }

    public static Func<T, object?>? GetValueGetter<T>(this PropertyInfo propertyInfo)
    {
        return StrongTypedCache<T>.PropertyValueGetters.GetOrAdd(propertyInfo, prop =>
        {
            if (!prop.CanRead)
                return null;

            var instance = Expression.Parameter(typeof(T), "i");
            var property = Expression.Property(instance, prop);
            var convert = Expression.TypeAs(property, typeof(object));
            return (Func<T, object>)Expression.Lambda(convert, instance).Compile();
        });
    }

    public static Func<object, object?>? GetValueGetter(this PropertyInfo propertyInfo)
    {
        return CacheUtil.PropertyValueGetters.GetOrAdd(propertyInfo, prop =>
        {
            if (!prop.CanRead)
                return null;

            Debug.Assert(propertyInfo.DeclaringType != null);

            var instance = Expression.Parameter(typeof(object), "obj");
            var getterCall = Expression.Call(propertyInfo.DeclaringType!.IsValueType
                ? Expression.Unbox(instance, propertyInfo.DeclaringType)
                : Expression.Convert(instance, propertyInfo.DeclaringType), prop.GetGetMethod()!);
            var castToObject = Expression.Convert(getterCall, typeof(object));
            return (Func<object, object>)Expression.Lambda(castToObject, instance).Compile();
        });
    }

    public static Action<T, object?>? GetValueSetter<T>(this PropertyInfo propertyInfo) where T : class
    {
        return StrongTypedCache<T>.PropertyValueSetters.GetOrAdd(propertyInfo, prop =>
        {
            if (!prop.CanWrite)
                return null;

            var instance = Expression.Parameter(typeof(T), "i");
            var argument = Expression.Parameter(typeof(object), "a");
            var setterCall = Expression.Call(instance, prop.GetSetMethod()!, Expression.Convert(argument, prop.PropertyType));
            return (Action<T, object?>)Expression.Lambda(setterCall, instance, argument).Compile();
        });
    }

    public static Action<object, object?>? GetValueSetter(this PropertyInfo propertyInfo)
    {
        Guard.NotNull(propertyInfo, nameof(propertyInfo));
        return CacheUtil.PropertyValueSetters.GetOrAdd(propertyInfo, prop =>
        {
            if (!prop.CanWrite)
                return null;

            var obj = Expression.Parameter(typeof(object), "o");
            var value = Expression.Parameter(typeof(object));

            // Note that we are using Expression.Unbox for value types and Expression.Convert for reference types
            var expr =
            Expression.Lambda<Action<object, object?>>(
                Expression.Call(
                    propertyInfo.DeclaringType!.IsValueType
                        ? Expression.Unbox(obj, propertyInfo.DeclaringType)
                        : Expression.Convert(obj, propertyInfo.DeclaringType),
                    propertyInfo.GetSetMethod()!,
                    Expression.Convert(value, propertyInfo.PropertyType)),
                obj, value);
            return expr.Compile();
        });
    }

    public static bool HasEmptyConstructor<T>(this T @this)
        => typeof(T).HasEmptyConstructor();

    /// <summary>
    /// 是否是 ValueTuple
    /// </summary>
    /// <returns></returns>
    public static bool IsValueTuple<T>(this T t)
        => typeof(T).IsValueTuple();

    /// <summary>
    /// 是否是值类型
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static bool IsValueType<T>(this T t)
        => typeof(T).IsValueType;

    /// <summary>
    ///     A T extension method that query if '@this' is array.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <returns>true if array, false if not.</returns>
    public static bool IsArray<T>(this T @this)
    {
        if (@this == null)
        {
            throw new ArgumentNullException(nameof(@this));
        }

        return @this.GetType().IsArray;
    }

    /// <summary>
    ///     A T extension method that query if '@this' is enum.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <returns>true if enum, false if not.</returns>
    public static bool IsEnum<T>(this T @this)
    {
        return typeof(T).IsEnum;
    }

    /// <summary>
    ///     A T extension method that query if '@this' is subclass of.
    /// </summary>
    /// <typeparam name="T">Generic type parameter.</typeparam>
    /// <param name="this">The @this to act on.</param>
    /// <param name="type">The Type to process.</param>
    /// <returns>true if subclass of, false if not.</returns>
    public static bool IsSubclassOf<T>(this T @this, Type type)
    {
        return typeof(T).IsSubclassOf(type);
    }

    /// <summary>
    ///     Retrieves a custom attribute applied to a specified assembly. Parameters specify the assembly and the type of
    ///     the custom attribute to search for.
    /// </summary>
    /// <param name="element">An object derived from the  class that describes a reusable collection of modules.</param>
    /// <param name="attributeType">The type, or a base type, of the custom attribute to search for.</param>
    /// <returns>
    ///     A reference to the single custom attribute of type  that is applied to , or null if there is no such
    ///     attribute.
    /// </returns>
    public static Attribute? GetCustomAttribute(this Assembly element, Type attributeType)
    {
        return Attribute.GetCustomAttribute(element, attributeType);
    }

    /// <summary>
    ///     Retrieves a custom attribute applied to an assembly. Parameters specify the assembly, the type of the custom
    ///     attribute to search for, and an ignored search option.
    /// </summary>
    /// <param name="element">An object derived from the  class that describes a reusable collection of modules.</param>
    /// <param name="attributeType">The type, or a base type, of the custom attribute to search for.</param>
    /// <param name="inherit">This parameter is ignored, and does not affect the operation of this method.</param>
    /// <returns>
    ///     A reference to the single custom attribute of type  that is applied to , or null if there is no such
    ///     attribute.
    /// </returns>
    public static Attribute? GetCustomAttribute(this Assembly element, Type attributeType, bool inherit)
    {
        return Attribute.GetCustomAttribute(element, attributeType, inherit);
    }

    /// <summary>
    ///     Retrieves an array of the custom attributes applied to an assembly. Parameters specify the assembly, and the
    ///     type of the custom attribute to search for.
    /// </summary>
    /// <param name="element">An object derived from the  class that describes a reusable collection of modules.</param>
    /// <param name="attributeType">The type, or a base type, of the custom attribute to search for.</param>
    /// <returns>
    ///     An  array that contains the custom attributes of type  applied to , or an empty array if no such custom
    ///     attributes exist.
    /// </returns>
    public static Attribute[] GetCustomAttributes(this Assembly element, Type attributeType)
    {
        return Attribute.GetCustomAttributes(element, attributeType);
    }

    /// <summary>
    ///     Retrieves an array of the custom attributes applied to an assembly. Parameters specify the assembly, the type
    ///     of the custom attribute to search for, and an ignored search option.
    /// </summary>
    /// <param name="element">An object derived from the  class that describes a reusable collection of modules.</param>
    /// <param name="attributeType">The type, or a base type, of the custom attribute to search for.</param>
    /// <param name="inherit">This parameter is ignored, and does not affect the operation of this method.</param>
    /// <returns>
    ///     An  array that contains the custom attributes of type  applied to , or an empty array if no such custom
    ///     attributes exist.
    /// </returns>
    public static Attribute[] GetCustomAttributes(this Assembly element, Type attributeType, bool inherit)
    {
        return Attribute.GetCustomAttributes(element, attributeType, inherit);
    }

    /// <summary>
    ///     Retrieves an array of the custom attributes applied to an assembly. A parameter specifies the assembly.
    /// </summary>
    /// <param name="element">An object derived from the  class that describes a reusable collection of modules.</param>
    /// <returns>
    ///     An  array that contains the custom attributes applied to , or an empty array if no such custom attributes
    ///     exist.
    /// </returns>
    public static Attribute[] GetCustomAttributes(this Assembly element)
    {
        return Attribute.GetCustomAttributes(element);
    }

    /// <summary>
    ///     Retrieves an array of the custom attributes applied to an assembly. Parameters specify the assembly, and an
    ///     ignored search option.
    /// </summary>
    /// <param name="element">An object derived from the  class that describes a reusable collection of modules.</param>
    /// <param name="inherit">This parameter is ignored, and does not affect the operation of this method.</param>
    /// <returns>
    ///     An  array that contains the custom attributes applied to , or an empty array if no such custom attributes
    ///     exist.
    /// </returns>
    public static Attribute[] GetCustomAttributes(this Assembly element, bool inherit)
    {
        return Attribute.GetCustomAttributes(element, inherit);
    }

    /// <summary>
    ///     Determines whether any custom attributes are applied to an assembly. Parameters specify the assembly, and the
    ///     type of the custom attribute to search for.
    /// </summary>
    /// <param name="element">An object derived from the  class that describes a reusable collection of modules.</param>
    /// <param name="attributeType">The type, or a base type, of the custom attribute to search for.</param>
    /// <returns>true if a custom attribute of type  is applied to ; otherwise, false.</returns>
    public static bool IsDefined(this Assembly element, Type attributeType)
    {
        return Attribute.IsDefined(element, attributeType);
    }

    /// <summary>
    ///     Determines whether any custom attributes are applied to an assembly. Parameters specify the assembly, the
    ///     type of the custom attribute to search for, and an ignored search option.
    /// </summary>
    /// <param name="element">An object derived from the  class that describes a reusable collection of modules.</param>
    /// <param name="attributeType">The type, or a base type, of the custom attribute to search for.</param>
    /// <param name="inherit">This parameter is ignored, and does not affect the operation of this method.</param>
    /// <returns>true if a custom attribute of type  is applied to ; otherwise, false.</returns>
    public static bool IsDefined(this Assembly element, Type attributeType, bool inherit)
    {
        return Attribute.IsDefined(element, attributeType, inherit);
    }
}
