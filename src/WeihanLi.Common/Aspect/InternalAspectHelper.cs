using System.Reflection;

namespace WeihanLi.Common.Aspect
{
    internal class MethodInvokeHelper
    {
        public static readonly MethodInfo GetInvocationReturnValueMethod =
            typeof(AspectInvocation).GetProperty("ReturnValue")?.GetMethod;

        public static readonly MethodInfo InvokeAspectDelegateMethod =
            typeof(AspectDelegate).GetMethod(nameof(AspectDelegate.Invoke));

        public static readonly MethodInfo GetBaseMethod =
            typeof(AspectExtensions).GetMethod(nameof(AspectExtensions.GetBaseMethod));

        public static readonly MethodInfo GetCurrentMethod =
            typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod));

        public static readonly ConstructorInfo AspectInvocationConstructor =
            typeof(AspectInvocation).GetConstructors()[0];
    }
}
