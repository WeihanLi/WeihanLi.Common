using System.Linq;
using System.Reflection;

namespace WeihanLi.Common.Aspect
{
    public static class AspectExtensions
    {
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
