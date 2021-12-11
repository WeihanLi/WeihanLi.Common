using System;

namespace WeihanLi.Common.DependencyInjection;

[AttributeUsage(AttributeTargets.Constructor, AllowMultiple = false, Inherited = false)]
public sealed class ServiceConstructorAttribute : Attribute
{
}
