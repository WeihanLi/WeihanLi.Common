using System;

namespace WeihanLi.Common.Aspect
{
    public sealed class AspectInvokeException : Exception
    {
        public IInvocation Invocation { get; }

        public AspectInvokeException(IInvocation invocation, Exception innerException) : base($"Invoke {invocation.ProxyMethod.Name} exception", innerException)
        {
            Invocation = invocation;
        }
    }
}
