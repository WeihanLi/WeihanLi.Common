using System;
using System.Collections.Generic;
using System.Reflection;

namespace WeihanLi.Common.Aspect
{
    public class FluentAspectOptions
    {
        internal readonly Dictionary<Func<MethodInfo, bool>, InterceptionConfiguration> _interceptionConfigurations = new Dictionary<Func<MethodInfo, bool>, InterceptionConfiguration>();

        public IInterceptionConfiguration Intercept(Func<MethodInfo, bool> predict)
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
    }
}
