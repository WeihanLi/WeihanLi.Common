using System;
using System.Collections.Generic;

namespace WeihanLi.Common.Aspect
{
    public class FluentAspectOptions
    {
        internal readonly Dictionary<Func<IInvocation, bool>, InterceptionConfiguration> _interceptionConfigurations = new Dictionary<Func<IInvocation, bool>, InterceptionConfiguration>();

        internal readonly HashSet<Func<IInvocation, bool>> _noInterceptionConfigurations = new HashSet<Func<IInvocation, bool>>();

        public bool NoIntercept(Func<IInvocation, bool> predict)
        {
            return _noInterceptionConfigurations.Add(predict);
        }

        public IInterceptionConfiguration Intercept(Func<IInvocation, bool> predict)
        {
            if (null == predict)
            {
                throw new ArgumentNullException(nameof(predict));
            }
            if (_interceptionConfigurations.TryGetValue
                (predict, out var interceptionConfiguration)
            )
            {
                return interceptionConfiguration;
            }
            interceptionConfiguration = new InterceptionConfiguration(new List<IInterceptor>(16));
            _interceptionConfigurations[predict] = interceptionConfiguration;
            return interceptionConfiguration;
        }

        public FluentAspectOptions()
        {
            // register built-in necessary interceptors
            this.InterceptAll().With<TryInvokeInterceptor>();
            this.Intercept<IDisposable>(m => m.Dispose())
                .With<DisposableInterceptor>();
        }
    }
}
