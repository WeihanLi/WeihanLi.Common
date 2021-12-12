using System.Reflection;

namespace WeihanLi.Common.Helpers;

public static class DelegateHelper
{
    private static readonly Type[] _funcMaker;
    private static readonly Type[] _actionMaker;

    static DelegateHelper()
    {
        _funcMaker = new Type[18];
        _funcMaker[0] = typeof(Func<>);
        _funcMaker[1] = typeof(Func<,>);
        _funcMaker[2] = typeof(Func<,,>);
        _funcMaker[3] = typeof(Func<,,,>);
        _funcMaker[4] = typeof(Func<,,,,>);
        _funcMaker[5] = typeof(Func<,,,,,>);
        _funcMaker[6] = typeof(Func<,,,,,,>);
        _funcMaker[7] = typeof(Func<,,,,,,,>);
        _funcMaker[8] = typeof(Func<,,,,,,,,>);
        _funcMaker[9] = typeof(Func<,,,,,,,,,>);
        _funcMaker[10] = typeof(Func<,,,,,,,,,,>);
        _funcMaker[11] = typeof(Func<,,,,,,,,,,,>);
        _funcMaker[12] = typeof(Func<,,,,,,,,,,,,>);
        _funcMaker[13] = typeof(Func<,,,,,,,,,,,,,>);
        _funcMaker[14] = typeof(Func<,,,,,,,,,,,,,,>);
        _funcMaker[15] = typeof(Func<,,,,,,,,,,,,,,,>);
        _funcMaker[16] = typeof(Func<,,,,,,,,,,,,,,,>);
        _funcMaker[17] = typeof(Func<,,,,,,,,,,,,,,,,>);

        _actionMaker = new Type[17];
        _actionMaker[0] = typeof(Action);
        _actionMaker[1] = typeof(Action<>);
        _actionMaker[2] = typeof(Action<,>);
        _actionMaker[3] = typeof(Action<,,>);
        _actionMaker[4] = typeof(Action<,,,>);
        _actionMaker[5] = typeof(Action<,,,,>);
        _actionMaker[6] = typeof(Action<,,,,,>);
        _actionMaker[7] = typeof(Action<,,,,,,>);
        _actionMaker[8] = typeof(Action<,,,,,,,>);
        _actionMaker[9] = typeof(Action<,,,,,,,,>);
        _actionMaker[10] = typeof(Action<,,,,,,,,,>);
        _actionMaker[11] = typeof(Action<,,,,,,,,,,>);
        _actionMaker[12] = typeof(Action<,,,,,,,,,,,>);
        _actionMaker[13] = typeof(Action<,,,,,,,,,,,,>);
        _actionMaker[14] = typeof(Action<,,,,,,,,,,,,,>);
        _actionMaker[15] = typeof(Action<,,,,,,,,,,,,,,>);
        _actionMaker[16] = typeof(Action<,,,,,,,,,,,,,,,>);
    }

    public static Delegate FromMethod(MethodInfo method, object? target = null)
    {
        if (null == method)
        {
            throw new ArgumentNullException(nameof(method));
        }

        Type delegateType = GetDelegateType(method);
        return method.CreateDelegate(delegateType, target);
    }

    public static Type GetDelegateType(MethodInfo method)
    {
        if (null == method)
        {
            throw new ArgumentNullException(nameof(method));
        }
        var isVoidMethod = method.ReturnType == typeof(void);
        var parameterTypes = method.GetParameters()
            .Select(p => p.ParameterType)
            .ToArray();

        Type delegateType;
        if (isVoidMethod)
        {
            if (parameterTypes.Length == 0)
            {
                delegateType = _actionMaker[0];
            }
            else
            {
                delegateType = _actionMaker[parameterTypes.Length]
                    .MakeGenericType(parameterTypes);
            }
        }
        else
        {
            var types = new Type[parameterTypes.Length + 1];
            Array.Copy(parameterTypes, types, parameterTypes.Length);
            types[parameterTypes.Length] = method.ReturnType;

            delegateType = _funcMaker[parameterTypes.Length]
                .MakeGenericType(types);
        }

        return delegateType;
    }

    public static Type GetDelegate(Type[]? parametersTypes = null, Type? returnType = null)
    {
        if (returnType == null || returnType == typeof(void))
        {
            return GetAction(parametersTypes);
        }

        return GetFunc(returnType, parametersTypes);
    }

    public static Type GetFunc(Type returnType, params Type[]? parametersTypes)
    {
        if (returnType == typeof(void))
        {
            return GetAction(parametersTypes);
        }

        if (parametersTypes == null || parametersTypes.Length == 0)
        {
            return _funcMaker[0].MakeGenericType(returnType);
        }

        var types = new Type[parametersTypes.Length + 1];
        Array.Copy(parametersTypes, types, parametersTypes.Length);
        types[parametersTypes.Length] = returnType;

        return _funcMaker[parametersTypes.Length]
            .MakeGenericType(types);
    }

    public static Type GetAction(params Type[]? parametersTypes)
    {
        if (parametersTypes == null || parametersTypes.Length == 0)
        {
            return _actionMaker[0];
        }

        return _actionMaker[parametersTypes.Length]
            .MakeGenericType(parametersTypes);
    }
}
