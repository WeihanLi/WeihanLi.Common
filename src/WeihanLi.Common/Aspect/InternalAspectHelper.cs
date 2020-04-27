using System.Reflection;

namespace WeihanLi.Common.Aspect
{
    internal class InternalAspectHelper
    {
        public static readonly MethodInfo GetInvocationReturnValueMethod =
            typeof(AspectInvocation).GetProperty("ReturnValue")?.GetMethod;
    }
}
