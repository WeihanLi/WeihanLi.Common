using System;
using System.Reflection;

namespace WeihanLi.Common.Aspect
{
    public class AspectInvokeException : Exception
    {
        public object Target { get; set; }

        public object ProxyTarget { get; set; }

        public MethodInfo Method { get; set; }

        public AspectInvokeException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
