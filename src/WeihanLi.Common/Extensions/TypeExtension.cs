using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using WeihanLi.Common;
using WeihanLi.Common.Helpers;

// ReSharper disable once CheckNamespace
namespace WeihanLi.Extensions;

public static class TypeExtension
{
    private static readonly Type[] BasicTypes =
    {
            typeof(bool),

            typeof(sbyte),
            typeof(byte),
            typeof(int),
            typeof(uint),
            typeof(short),
            typeof(ushort),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),

            typeof(Guid),

            typeof(DateTime),// IsPrimitive:False
            typeof(TimeSpan),// IsPrimitive:False
            typeof(DateTimeOffset),

            typeof(char),
            typeof(string),// IsPrimitive:False

#if NET6_0_OR_GREATER
            typeof(DateOnly),
            typeof(TimeOnly),
#endif

            //typeof(object),// IsPrimitive:False
        };

    public static TypeCode GetTypeCode(this Type type) => Type.GetTypeCode(type);

    public static bool IsValueTuple(this Type type)
            => type.IsValueType && type.FullName?.StartsWith("System.ValueTuple`", StringComparison.Ordinal) == true;

    /// <summary>
    /// GetDescription
    /// </summary>
    /// <param name="type">type</param>
    /// <returns></returns>
    public static string? GetDescription(this Type type) =>
        type.GetCustomAttribute<DescriptionAttribute>()?.Description;

    /// <summary>
    /// 判断是否基元类型，如果是可空类型会先获取里面的类型，如 int? 也是基元类型
    /// The primitive types are Boolean, Byte, SByte, Int16, UInt16, Int32, UInt32, Int64, UInt64, IntPtr, UIntPtr, Char, Double, and Single.
    /// </summary>
    /// <param name="type">type</param>
    /// <returns></returns>
    public static bool IsPrimitiveType(this Type type)
        => (Nullable.GetUnderlyingType(type) ?? type).IsPrimitive;

    public static bool IsPrimitiveType<T>() => IsPrimitiveType(typeof(T));

    public static bool IsBasicType(this Type type)
    {
        var unWrappedType = type.Unwrap();
        return unWrappedType.IsEnum || BasicTypes.Contains(unWrappedType);
    }

    public static bool IsBasicType<T>() => IsBasicType(typeof(T));

    public static bool HasNamespace(this Type type) => Guard.NotNull(type).Namespace != null;

    /// <summary>
    /// Finds best constructor, least parameter
    /// </summary>
    /// <param name="type">type</param>
    /// <param name="parameterTypes"></param>
    /// <returns>Matching constructor or default one</returns>
    [RequiresUnreferencedCode("Unreferenced code may be used")]
    public static ConstructorInfo? GetConstructor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] this Type type, params Type[]? parameterTypes)
    {
        if (parameterTypes == null || parameterTypes.Length == 0)
            return GetEmptyConstructor(type);

        ActivatorHelper.FindApplicableConstructor(type, parameterTypes, out var ctor, out _);
        return ctor;
    }

    public static ConstructorInfo? GetEmptyConstructor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] this Type type)
    {
        var constructors = type.GetConstructors();

        var ctor = constructors.OrderBy(c => c.IsPublic ? 0 : (c.IsPrivate ? 2 : 1))
            .ThenBy(c => c.GetParameters().Length).FirstOrDefault();

        return ctor?.GetParameters().Length == 0 ? ctor : null;
    }

    /// <summary>
    /// Determines whether this type is assignable to <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type to test assignability to.</typeparam>
    /// <param name="this">The type to test.</param>
    /// <returns>True if this type is assignable to references of type
    /// <typeparamref name="T"/>; otherwise, False.</returns>
    public static bool IsAssignableTo<T>(this Type @this)
    {
        Guard.NotNull(@this);
        return typeof(T).IsAssignableFrom(@this);
    }

    /// <summary>
    /// Finds a constructor with the matching type parameters.
    /// </summary>
    /// <param name="type">The type being tested.</param>
    /// <param name="constructorParameterTypes">The types of the contractor to find.</param>
    /// <returns>The <see cref="ConstructorInfo"/> is a match is found; otherwise, <c>null</c>.</returns>
    public static ConstructorInfo? GetMatchingConstructor([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] this Type type, Type[]? constructorParameterTypes)
    {
        if (constructorParameterTypes == null || constructorParameterTypes.Length == 0)
            return GetEmptyConstructor(type);

        return type.GetConstructors()
            .FirstOrDefault(c => c.GetParameters()
                .Select(p => p.ParameterType)
                .SequenceEqual(constructorParameterTypes)
            );
    }

    /// <summary>
    /// Get ImplementedInterfaces
    /// </summary>
    /// <param name="type">type</param>
    /// <returns></returns>
    public static IEnumerable<Type> GetImplementedInterfaces([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces)] this Type type)
    {
        return type.GetTypeInfo().ImplementedInterfaces;
    }
}
