using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Aspect
{
    public abstract class AbstractInterceptor : Attribute, IInterceptor
    {
        public abstract Task Invoke(IInvocation invocation, Func<Task> next);
    }

    public class AttributeInterceptorResolver : IInterceptorResolver
    {
        public IReadOnlyCollection<IInterceptor> ResolveInterceptors(IInvocation invocation)
        {
            if (null == invocation)
            {
                return ArrayHelper.Empty<IInterceptor>();
            }

            var list = new List<IInterceptor>(16);
            if (invocation.Method != null)
            {
                foreach (var interceptor in invocation.Method.GetCustomAttributes<AbstractInterceptor>())
                {
                    list.Add(interceptor);
                }
            }
            else
            {
                foreach (var interceptor in invocation.ProxyMethod.GetCustomAttributes<AbstractInterceptor>())
                {
                    list.Add(interceptor);
                }
            }

            var parameterTypes = invocation.ProxyMethod.GetParameters().Select(x => x.ParameterType).ToArray();
            foreach (var @interface in invocation.ProxyTarget.GetType().GetImplementedInterfaces())
            {
                var interfaceMethod = @interface.GetMethod(invocation.ProxyMethod.Name, parameterTypes);
                if (null != interfaceMethod)
                {
                    foreach (var interceptor in interfaceMethod.GetCustomAttributes<AbstractInterceptor>())
                    {
                        if (!list.Exists(x => x.GetType() == interceptor.GetType()))
                        {
                            list.Add(interceptor);
                        }
                    }
                }
            }

            return list;
        }
    }
}
