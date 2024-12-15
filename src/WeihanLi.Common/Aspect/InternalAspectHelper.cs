using System.Reflection;

namespace WeihanLi.Common.Aspect;

internal static class MethodInvokeHelper
{
    public static readonly MethodInfo GetInvocationReturnValueMethod =
        typeof(AspectInvocation).GetProperty("ReturnValue")!.GetGetMethod()!;

    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
    public static readonly MethodInfo InvokeAspectDelegateMethod =
        typeof(AspectDelegate).GetMethod(nameof(AspectDelegate.Invoke))!;

    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
    public static readonly MethodInfo GetCurrentMethod =
        typeof(MethodBase).GetMethod(nameof(MethodBase.GetCurrentMethod))!;

    [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
    public static readonly ConstructorInfo AspectInvocationConstructor =
        typeof(AspectInvocation).GetConstructors()[0];
}
