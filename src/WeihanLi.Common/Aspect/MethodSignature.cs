using System.Reflection;

namespace WeihanLi.Common.Aspect;

internal sealed class MethodSignature(string methodName, IReadOnlyList<Type> parameters)
{
    public IReadOnlyList<Type> Parameters { get; } = parameters;
    public string MethodName { get; } = methodName;

    public MethodSignature(MethodBase method) : this(method.Name, method.GetParameters().Select(p => p.ParameterType).ToArray())
    {
    }

    public override bool Equals(object? obj)
    {
        // if object is null or type does not match return false...
        if (obj is not MethodSignature signature)
            return false;

        // compares method name...
        if (MethodName != signature.MethodName)
            return false;

        // compares params ...
        if (Parameters.Count != signature.Parameters.Count)
            return false;

        // compares params types...
        for (var i = 0; i < Parameters.Count; i++)
        {
            if (!Parameters[i].IsGenericParameter && Parameters[i] != signature.Parameters[i])
            {
                return false;
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        // XORs members...
        var hash = MethodName.GetHashCode();
        return Parameters.Aggregate(hash, (a, b) => a ^ b.GetHashCode());
    }
}

internal static class MethodSignatureExtensions
{
    public static MethodSignature GetSignature(this MethodBase method)
    {
        return new(method);
    }
}
