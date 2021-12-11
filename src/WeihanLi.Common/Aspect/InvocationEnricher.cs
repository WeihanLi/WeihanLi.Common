using System;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Aspect;

public interface IInvocationEnricher : IEnricher<IInvocation>
{
}

public sealed class PropertyInvocationEnricher : PropertyEnricher<IInvocation>, IInvocationEnricher
{
    public PropertyInvocationEnricher(string propertyName, object propertyValue, bool overwrite = false) : base(propertyName, propertyValue, overwrite)
    {
    }

    public PropertyInvocationEnricher(string propertyName, Func<IInvocation, object> propertyValueFactory, bool overwrite = false) : base(propertyName, propertyValueFactory, overwrite)
    {
    }

    public PropertyInvocationEnricher(string propertyName, Func<IInvocation, object> propertyValueFactory, Func<IInvocation, bool> propertyPredict, bool overwrite = false) : base(propertyName, propertyValueFactory, propertyPredict, overwrite)
    {
    }

    protected override Action<IInvocation, string, Func<IInvocation, object>, bool> EnrichAction =>
        (invocation, propertyName, propertyValueFactory, overwrite) =>
            invocation.AddProperty(propertyName, propertyValueFactory, overwrite);
}
