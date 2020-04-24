using System.Collections.Generic;

namespace WeihanLi.Common.Aspect
{
    public interface IInterceptionConfiguration
    {
        IInterceptionConfiguration With(params IInterceptor[] interceptors);
    }

    internal class InterceptionConfiguration : IInterceptionConfiguration
    {
        public List<IInterceptor> Interceptors { get; }

        public InterceptionConfiguration(List<IInterceptor> interceptors)
        {
            Interceptors = interceptors;
        }

        public IInterceptionConfiguration With(params IInterceptor[] interceptors)
        {
            if (interceptors != null)
            {
                foreach (var interceptor in interceptors)
                {
                    Interceptors.Add(interceptor);
                }
            }

            return this;
        }
    }
}
