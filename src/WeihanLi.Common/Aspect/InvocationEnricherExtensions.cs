using System;

namespace WeihanLi.Common.Aspect
{
    public static class InvocationEnricherExtensions
    {
        public static void AddProperty(this IInvocation invocation, string propertyName,
            object propertyValue, bool overwrite = false)
        {
            if (null == invocation)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            if (!invocation.Properties.ContainsKey(propertyName) || overwrite)
            {
                invocation.Properties[propertyName] = propertyValue;
            }
        }

        public static void AddProperty(this IInvocation invocation, string propertyName,
            Func<IInvocation, object> propertyValueFactory, bool overwrite = false)
        {
            if (null == invocation)
            {
                throw new ArgumentNullException(nameof(invocation));
            }

            if (!invocation.Properties.ContainsKey(propertyName)
                || overwrite)
            {
                invocation.Properties[propertyName] = propertyValueFactory?.Invoke(invocation);
            }
        }
    }
}
