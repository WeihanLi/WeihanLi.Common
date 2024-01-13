namespace WeihanLi.Common.Aspect;

public static class InvocationEnricherExtensions
{
    public static void AddProperty(this IInvocation invocation, string propertyName,
        object propertyValue, bool overwrite = false)
    {
        Guard.NotNull(invocation);

        if (!invocation.Properties.ContainsKey(propertyName) || overwrite)
        {
            invocation.Properties[propertyName] = propertyValue;
        }
    }

    public static void AddProperty(this IInvocation invocation, string propertyName,
        Func<IInvocation, object?> propertyValueFactory, bool overwrite = false)
    {
        Guard.NotNull(invocation);

        if (!invocation.Properties.ContainsKey(propertyName)
            || overwrite)
        {
            invocation.Properties[propertyName]
                = Guard.NotNull(propertyValueFactory, nameof(propertyValueFactory)).Invoke(invocation);
        }
    }
}
