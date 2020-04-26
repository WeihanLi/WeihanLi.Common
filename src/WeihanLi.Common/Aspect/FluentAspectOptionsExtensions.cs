using System;
using System.Linq.Expressions;
using System.Reflection;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Aspect
{
    public static class FluentAspectOptionsExtensions
    {
        public static IInterceptionConfiguration InterceptAll(this FluentAspectOptions options)
            => options.Intercept(m => true);

        public static IInterceptionConfiguration Intercept<T>(this FluentAspectOptions options,
            Expression<Func<T, object>> method)
        {
            return options.Intercept<T>(method.GetMethodExpression());
        }

        public static IInterceptionConfiguration Intercept<T>(this FluentAspectOptions options,
            Expression<Action<T>> method)
        {
            return options.Intercept<T>(method.GetMethodExpression());
        }

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

        public static IInterceptionConfiguration Intercept(this FluentAspectOptions options,
            Func<MethodInfo, bool> methodPredict)
        {
            return options.Intercept(invocation => methodPredict(invocation.Method ?? invocation.ProxyMethod));
        }

        public static IInterceptionConfiguration Intercept<T>(this FluentAspectOptions options,
            MethodCallExpression methodCallExpression)
        {
            var innerMethod = methodCallExpression?.Method;
            if (null == innerMethod)
            {
                throw new InvalidOperationException($"no method found");
            }
            Expression<Func<MethodInfo, bool>> expression = m => m.DeclaringType.IsAssignableTo<T>();
            var methodSignature = innerMethod.GetSignature();
            expression = expression.And(m => m.GetSignature().Equals(methodSignature));
            return options.Intercept(expression.Compile());
        }

        public static FluentAspectOptions NoIntercept<T>(this FluentAspectOptions options,
            Expression<Func<T, object>> method)
        {
            options.NoIntercept<T>(method.GetMethodExpression());
            return options;
        }

        public static FluentAspectOptions NoIntercept<T>(this FluentAspectOptions options,
            Expression<Action<T>> method)
        {
            options.NoIntercept<T>(method.GetMethodExpression());
            return options;
        }

        public static FluentAspectOptions NoIntercept<T>(this FluentAspectOptions options,
            Expression<Func<MethodInfo, bool>> andExpression = null)
        {
            Expression<Func<MethodInfo, bool>> expression = m => m.DeclaringType.IsAssignableTo<T>();
            if (null != andExpression)
            {
                expression = expression.And(andExpression);
            }
            options.Intercept(expression.Compile());
            return options;
        }

        public static FluentAspectOptions NoIntercept<T>(this FluentAspectOptions options,
            MethodCallExpression methodCallExpression)
        {
            var innerMethod = methodCallExpression?.Method;
            if (null == innerMethod)
            {
                throw new InvalidOperationException($"no method found");
            }
            Expression<Func<MethodInfo, bool>> expression = m => m.DeclaringType.IsAssignableTo<T>();
            var methodSignature = innerMethod.GetSignature();
            expression = expression.And(m => m.GetSignature().Equals(methodSignature));
            return options.NoIntercept(expression.Compile());
        }

        public static FluentAspectOptions NoIntercept(this FluentAspectOptions options,
            Func<MethodInfo, bool> methodPredict)
        {
            options.NoIntercept(invocation => methodPredict(invocation.Method ?? invocation.ProxyMethod));
            return options;
        }
    }
}
