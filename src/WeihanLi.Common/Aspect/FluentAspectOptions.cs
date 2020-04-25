using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using WeihanLi.Extensions;

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

        public FluentAspectOptions()
        {
            // register built-in necessary interceptors
            this.InterceptAll().With<TryInvokeInterceptor>();
            this.Intercept<IDisposable>(m => m.Name == nameof(IDisposable.Dispose))
                .With<DisposableInterceptor>();
        }
    }

    public static class FluentAspectOptionsExtensions
    {
        public static IInterceptionConfiguration InterceptAll(this FluentAspectOptions options)
            => options.Intercept(m => true);

        public static IInterceptionConfiguration Intercept<T>(this FluentAspectOptions options,
            Expression<Func<MethodInfo, bool>> andExpression = null)
        {
            Expression<Func<MethodInfo, bool>> expression = m => m.DeclaringType.IsAssignableTo<T>();
            if (null != andExpression)
            {
                expression = expression.And(andExpression);
            }
            return options.Intercept(expression.Compile());
        }
    }
}
