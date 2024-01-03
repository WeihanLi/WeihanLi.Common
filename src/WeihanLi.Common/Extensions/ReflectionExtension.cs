using System.ComponentModel;
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
    public static MethodInfo? GetMethodBySignature([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] this Type type, MethodInfo method)
    {
        Guard.NotNull(type);
        Guard.NotNull(method);

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

    [RequiresUnreferencedCode("Unreferenced code may be used.")]
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
