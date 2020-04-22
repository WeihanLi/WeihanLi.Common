using System.Reflection;

namespace WeihanLi.Common.Aspect
{
    public interface IInvocation
    {
        public MethodInfo ProxyMethod { get; }

        public object ProxyTarget { get; }
        public MethodInfo Method { get; }

        public object Target { get; }

        public object[] Parameters { get; }

        public object ReturnValue { get; set; }
    }

    public class MethodInvocation : IInvocation
    {
        public MethodInfo ProxyMethod { get; }

        public MethodInfo Method { get; }

        public object ProxyTarget { get; }

        public object Target { get; }

        public object[] Parameters { get; }

        public object ReturnValue { get; set; }

        public MethodInvocation(MethodInfo method, MethodInfo methodBase, object proxyTarget, object target, object[] parameters)
        {
            ProxyMethod = method;
            Method = methodBase;
            ProxyTarget = proxyTarget;
            Target = target;
            Parameters = parameters;
        }
    }
}
