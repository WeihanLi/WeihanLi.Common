using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Helpers;

public static class DelegateHelper
{
    private static readonly Type[] _funcFactory;
    private static readonly Type[] _actionFactory;

    static DelegateHelper()
    {
        _funcFactory = new Type[18];
        _funcFactory[0] = typeof(Func<>);
        _funcFactory[1] = typeof(Func<,>);
        _funcFactory[2] = typeof(Func<,,>);
        _funcFactory[3] = typeof(Func<,,,>);
        _funcFactory[4] = typeof(Func<,,,,>);
        _funcFactory[5] = typeof(Func<,,,,,>);
        _funcFactory[6] = typeof(Func<,,,,,,>);
        _funcFactory[7] = typeof(Func<,,,,,,,>);
        _funcFactory[8] = typeof(Func<,,,,,,,,>);
        _funcFactory[9] = typeof(Func<,,,,,,,,,>);
        _funcFactory[10] = typeof(Func<,,,,,,,,,,>);
        _funcFactory[11] = typeof(Func<,,,,,,,,,,,>);
        _funcFactory[12] = typeof(Func<,,,,,,,,,,,,>);
        _funcFactory[13] = typeof(Func<,,,,,,,,,,,,,>);
        _funcFactory[14] = typeof(Func<,,,,,,,,,,,,,,>);
        _funcFactory[15] = typeof(Func<,,,,,,,,,,,,,,,>);
        _funcFactory[16] = typeof(Func<,,,,,,,,,,,,,,,>);
        _funcFactory[17] = typeof(Func<,,,,,,,,,,,,,,,,>);

        _actionFactory = new Type[17];
        _actionFactory[0] = typeof(Action);
        _actionFactory[1] = typeof(Action<>);
        _actionFactory[2] = typeof(Action<,>);
        _actionFactory[3] = typeof(Action<,,>);
        _actionFactory[4] = typeof(Action<,,,>);
        _actionFactory[5] = typeof(Action<,,,,>);
        _actionFactory[6] = typeof(Action<,,,,,>);
        _actionFactory[7] = typeof(Action<,,,,,,>);
        _actionFactory[8] = typeof(Action<,,,,,,,>);
        _actionFactory[9] = typeof(Action<,,,,,,,,>);
        _actionFactory[10] = typeof(Action<,,,,,,,,,>);
        _actionFactory[11] = typeof(Action<,,,,,,,,,,>);
        _actionFactory[12] = typeof(Action<,,,,,,,,,,,>);
        _actionFactory[13] = typeof(Action<,,,,,,,,,,,,>);
        _actionFactory[14] = typeof(Action<,,,,,,,,,,,,,>);
        _actionFactory[15] = typeof(Action<,,,,,,,,,,,,,,>);
        _actionFactory[16] = typeof(Action<,,,,,,,,,,,,,,,>);
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public static Delegate FromMethod(MethodInfo method, object? target = null)
    {
        Guard.NotNull(method);
        var delegateType = GetDelegateType(method);
        return method.CreateDelegate(delegateType, target);
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public static Type GetDelegateType(MethodInfo method)
    {
        Guard.NotNull(method);
        var isVoidMethod = method.ReturnType == typeof(void);
        var parameterTypes = method.GetParameters()
            .Select(p => p.ParameterType)
            .ToArray();

        Type delegateType;
        if (isVoidMethod)
        {
            if (parameterTypes.Length == 0)
            {
                delegateType = _actionFactory[0];
            }
            else
            {
                delegateType = _actionFactory[parameterTypes.Length]
                    .MakeGenericType(parameterTypes);
            }
        }
        else
        {
            var types = new Type[parameterTypes.Length + 1];
            Array.Copy(parameterTypes, types, parameterTypes.Length);
            types[parameterTypes.Length] = method.ReturnType;

            delegateType = _funcFactory[parameterTypes.Length]
                .MakeGenericType(types);
        }

        return delegateType;
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public static Type GetDelegate(Type[]? parametersTypes = null, Type? returnType = null)
    {
        if (returnType == null || returnType == typeof(void))
        {
            return GetAction(parametersTypes);
        }

        return GetFunc(returnType, parametersTypes);
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public static Type GetFunc(Type returnType, params Type[]? parametersTypes)
    {
        if (returnType == typeof(void))
        {
            return GetAction(parametersTypes);
        }

        if (parametersTypes.IsNullOrEmpty())
        {
            return _funcFactory[0].MakeGenericType(returnType);
        }

        var types = new Type[parametersTypes.Length + 1];
        Array.Copy(parametersTypes, types, parametersTypes.Length);
        types[parametersTypes.Length] = returnType;

        return _funcFactory[parametersTypes.Length]
            .MakeGenericType(types);
    }

    [RequiresDynamicCode("The native code for this instantiation might not be available at runtime.")]
    [RequiresUnreferencedCode("If some of the generic arguments are annotated (either with DynamicallyAccessedMembersAttribute, or generic constraints), trimming can't validate that the requirements of those annotations are met.")]
    public static Type GetAction(params Type[]? parametersTypes)
    {
        if (parametersTypes == null || parametersTypes.Length == 0)
        {
            return _actionFactory[0];
        }

        return _actionFactory[parametersTypes.Length]
            .MakeGenericType(parametersTypes);
    }
}
