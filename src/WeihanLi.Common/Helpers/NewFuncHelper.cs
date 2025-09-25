using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace WeihanLi.Common.Helpers;

[RequiresDynamicCode("Expression compilation requires dynamic code generation.")]
[RequiresUnreferencedCode("Unreferenced code may be used")]
public static class NewFuncHelper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>
{
    /// <summary>
    /// CreateNewInstance func
    /// T need to have a parameter less constructor
    /// </summary>
    public static readonly Func<T> Instance = Expression.Lambda<Func<T>>
    (
        Expression.New(typeof(T))
    ).Compile();
}
