using System;

namespace WeihanLi.Common.DependencyInjection
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = false)]
    public sealed class FromServiceAttribute : Attribute
    {
    }
}
