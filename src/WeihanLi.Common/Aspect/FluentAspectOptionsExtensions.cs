using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using WeihanLi.Common.Helpers;
using WeihanLi.Extensions;

namespace WeihanLi.Common.Aspect;

public static class FluentAspectOptionsExtensions
{
    #region Intercept

    public static IInterceptionConfiguration Intercept(this FluentAspectOptions options,
        Func<IInvocation, bool> predict)
    {
        if (null == predict)
        {
            throw new ArgumentNullException(nameof(predict));
        }

        if (options.InterceptionConfigurations.TryGetValue
            (predict, out var interceptionConfiguration))
        {
            return interceptionConfiguration;
        }

        interceptionConfiguration = new InterceptionConfiguration();
        options.InterceptionConfigurations[predict] = interceptionConfiguration;
        return interceptionConfiguration;
    }

    public static IInterceptionConfiguration InterceptAll(this FluentAspectOptions options)
        => options.Intercept(_ => true);

    public static IInterceptionConfiguration InterceptType(this FluentAspectOptions options,
        Func<Type, bool> typesFilter)
    {
        if (null == typesFilter)
        {
            throw new ArgumentNullException(nameof(typesFilter));
        }

        return options.InterceptMethod(m => typesFilter(m.DeclaringType!));
    }

    public static IInterceptionConfiguration InterceptMethod<T>(this FluentAspectOptions options,
        Expression<Func<T, object>> method)
    {
        return options.InterceptMethod<T>(method.GetMethodExpression());
    }

    public static IInterceptionConfiguration InterceptMethod<T>(this FluentAspectOptions options,
        MethodInfo method)
    {
        if (null == method)
        {
            throw new ArgumentNullException(nameof(method));
        }

        var methodSignature = method.GetSignature();
        return options.InterceptMethod<T>(m => m.GetSignature().Equals(methodSignature));
    }

    public static IInterceptionConfiguration InterceptMethod(this FluentAspectOptions options,
        MethodInfo method)
    {
        if (null == method)
        {
            throw new ArgumentNullException(nameof(method));
        }

        var methodSignature = method.GetSignature();
        return options.InterceptMethod(m => m.GetSignature().Equals(methodSignature));
    }

    public static IInterceptionConfiguration InterceptPropertyGetter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]T>(this FluentAspectOptions options,
        Expression<Func<T, object>> expression)
    {
        var prop = expression.GetProperty();
        if (null == prop)
        {
            throw new InvalidOperationException("no property found");
        }

        if (!prop.CanRead || prop.GetMethod == null)
        {
            throw new InvalidOperationException($"the property {prop.Name} can not read");
        }

        return options.InterceptMethod<T>(prop.GetMethod);
    }

    public static IInterceptionConfiguration InterceptPropertySetter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]T>(this FluentAspectOptions options,
        Expression<Func<T, object>> expression)
    {
        var prop = expression.GetProperty();
        if (null == prop)
        {
            throw new InvalidOperationException("no property found");
        }

        if (!prop.CanWrite || prop.SetMethod == null)
        {
            throw new InvalidOperationException($"the property {prop.Name} can not write");
        }

        return options.InterceptMethod<T>(prop.SetMethod);
    }

    public static IInterceptionConfiguration InterceptMethod<T>(this FluentAspectOptions options,
        Expression<Action<T>> method)
    {
        return options.InterceptMethod<T>(method.GetMethodExpression());
    }

    public static IInterceptionConfiguration InterceptType<T>(this FluentAspectOptions options)
    {
        return options.InterceptMethod(m => m.DeclaringType!.IsAssignableTo<T>());
    }

    public static IInterceptionConfiguration InterceptMethod<T>(this FluentAspectOptions options,
        Expression<Func<MethodInfo, bool>> andExpression)
    {
        Expression<Func<MethodInfo, bool>> expression = m => m.DeclaringType!.IsAssignableTo<T>();
        expression = expression.And(Guard.NotNull(andExpression, nameof(andExpression)));
        return options.InterceptMethod(expression.Compile());
    }

    public static IInterceptionConfiguration InterceptMethod(this FluentAspectOptions options,
        Func<MethodInfo, bool> methodPredict)
    {
        return options.Intercept(invocation => methodPredict(invocation.Method ?? invocation.ProxyMethod));
    }

    public static IInterceptionConfiguration InterceptMethod<T>(this FluentAspectOptions options,
        MethodCallExpression methodCallExpression)
    {
        var innerMethod = methodCallExpression.Method;
        if (null == innerMethod)
        {
            throw new InvalidOperationException("no method found");
        }

        Expression<Func<MethodInfo, bool>> expression = m => m.DeclaringType!.IsAssignableTo<T>();
        var methodSignature = innerMethod.GetSignature();
        expression = expression.And(m => m.GetSignature().Equals(methodSignature));
        return options.InterceptMethod(expression.Compile());
    }

    #endregion Intercept

    #region NoIntercept

    public static bool NoIntercept(this FluentAspectOptions options, Func<IInvocation, bool> predict)
    {
        return options.NoInterceptionConfigurations.Add(predict);
    }

    public static FluentAspectOptions NoInterceptMethod<T>(this FluentAspectOptions options,
        Expression<Func<T, object>> method)
    {
        options.NoInterceptMethod<T>(method.GetMethodExpression());
        return options;
    }

    public static FluentAspectOptions NoInterceptMethod<T>(this FluentAspectOptions options,
        Expression<Action<T>> method)
    {
        options.NoInterceptMethod<T>(method.GetMethodExpression());
        return options;
    }

    public static FluentAspectOptions NoInterceptType(this FluentAspectOptions options,
        Func<Type, bool> typesFilter)
    {
        Guard.NotNull(options, nameof(options));
        Guard.NotNull(typesFilter, nameof(typesFilter));
        options.NoInterceptMethod(m => typesFilter(m.DeclaringType!));
        return options;
    }

    public static FluentAspectOptions NoInterceptType<T>(this FluentAspectOptions options)
    {
        options.NoInterceptMethod(m => m.DeclaringType!.IsAssignableTo<T>());
        return options;
    }

    public static FluentAspectOptions NoInterceptMethod<T>(this FluentAspectOptions options,
        Expression<Func<MethodInfo, bool>> andExpression)
    {
        Expression<Func<MethodInfo, bool>> expression = m => m.DeclaringType!.IsAssignableTo<T>();
        expression = expression.And(Guard.NotNull(andExpression, nameof(andExpression)));
        options.NoInterceptMethod(expression.Compile());
        return options;
    }

    public static FluentAspectOptions NoInterceptMethod<T>(this FluentAspectOptions options,
        MethodCallExpression methodCallExpression)
    {
        var innerMethod = methodCallExpression.Method;
        if (null == innerMethod)
        {
            throw new InvalidOperationException($"no method found");
        }

        Expression<Func<MethodInfo, bool>> expression = m => m.DeclaringType!.IsAssignableTo<T>();
        var methodSignature = innerMethod.GetSignature();
        expression = expression.And(m => m.GetSignature().Equals(methodSignature));
        return options.NoInterceptMethod(expression.Compile());
    }

    public static FluentAspectOptions NoInterceptMethod(this FluentAspectOptions options,
        Func<MethodInfo, bool> methodPredict)
    {
        options.NoIntercept(invocation => methodPredict(invocation.Method ?? invocation.ProxyMethod));
        return options;
    }

    public static FluentAspectOptions NoInterceptMethod<T>(this FluentAspectOptions options,
        MethodInfo method)
    {
        if (null == method)
        {
            throw new ArgumentNullException(nameof(method));
        }

        var methodSignature = method.GetSignature();
        return options.NoInterceptMethod<T>(m => m.GetSignature().Equals(methodSignature));
    }

    public static FluentAspectOptions NoInterceptMethod(this FluentAspectOptions options,
        MethodInfo method)
    {
        if (null == method)
        {
            throw new ArgumentNullException(nameof(method));
        }

        var methodSignature = method.GetSignature();
        return options.NoInterceptMethod(m => m.GetSignature().Equals(methodSignature));
    }

    public static FluentAspectOptions NoInterceptProperty<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]T>(this FluentAspectOptions options,
        Expression<Func<T, object>> expression)
    {
        var prop = expression.GetProperty();
        if (null == prop)
        {
            throw new InvalidOperationException("no property found");
        }

        if (prop.GetMethod != null)
        {
            options = options.NoInterceptMethod<T>(prop.GetMethod);
        }
        if (prop.SetMethod != null)
        {
            options = options.NoInterceptMethod<T>(prop.SetMethod);
        }
        return options;
    }

    public static FluentAspectOptions NoInterceptPropertyGetter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]T>(this FluentAspectOptions options,
        Expression<Func<T, object>> expression)
    {
        var prop = expression.GetProperty();
        if (null == prop)
        {
            throw new InvalidOperationException("no property found");
        }

        if (!prop.CanRead || prop.GetMethod == null)
        {
            throw new InvalidOperationException($"the property {prop.Name} can not read");
        }

        return options.NoInterceptMethod<T>(prop.GetMethod);
    }

    public static FluentAspectOptions NoInterceptPropertySetter<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]T>(this FluentAspectOptions options,
        Expression<Func<T, object>> expression)
    {
        var prop = expression.GetProperty();
        if (null == prop)
        {
            throw new InvalidOperationException("no property found");
        }

        if (!prop.CanWrite || prop.SetMethod == null)
        {
            throw new InvalidOperationException($"the property {prop.Name} can not write");
        }

        return options.NoInterceptMethod<T>(prop.SetMethod);
    }

    #endregion NoIntercept

    #region InterceptorResolver

    public static FluentAspectOptions UseInterceptorResolver(this FluentAspectOptions options,
        IInterceptorResolver resolver)
    {
        Guard.NotNull(resolver);
        options.InterceptorResolver = resolver;
        return options;
    }

    public static FluentAspectOptions UseInterceptorResolver<TResolver>(this FluentAspectOptions options)
        where TResolver : IInterceptorResolver, new()
    {
        options.InterceptorResolver = new TResolver();
        return options;
    }

    #endregion InterceptorResolver

    #region ProxyFactory

    public static FluentAspectOptions UseProxyFactory(this FluentAspectOptions options, IProxyFactory proxyFactory)
    {
        options.ProxyFactory = proxyFactory;
        return options;
    }

    public static FluentAspectOptions UseProxyFactory<TProxyFactory>(this FluentAspectOptions options)
        where TProxyFactory : class, IProxyFactory, new()
    {
        options.ProxyFactory = new TProxyFactory();
        return options;
    }
    
    public static FluentAspectOptions UseProxyFactory<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)]TProxyFactory>(this FluentAspectOptions options,
        params object[] parameters) where TProxyFactory : class, IProxyFactory
    {
        options.ProxyFactory = ActivatorHelper.CreateInstance<TProxyFactory>(parameters);
        return options;
    }

    #endregion ProxyFactory

    #region Enricher

    public static FluentAspectOptions WithProperty(this FluentAspectOptions options, string propertyName,
        object propertyValue, bool overwrite = false)
    {
        options.Enrichers.Add(new PropertyInvocationEnricher(propertyName, propertyValue, overwrite));
        return options;
    }

    public static FluentAspectOptions WithProperty(this FluentAspectOptions options, string propertyName,
        Func<IInvocation, object> propertyValueFactory, bool overwrite = false)
    {
        options.Enrichers.Add(new PropertyInvocationEnricher(propertyName, propertyValueFactory, overwrite));
        return options;
    }

    public static FluentAspectOptions WithEnricher<TEnricher>(this FluentAspectOptions options) where TEnricher : IInvocationEnricher, new()
    {
        options.Enrichers.Add(new TEnricher());
        return options;
    }

    public static FluentAspectOptions WithEnricher<TEnricher>(this FluentAspectOptions options, TEnricher enricher) where TEnricher : IInvocationEnricher
    {
        options.Enrichers.Add(enricher);
        return options;
    }

    #endregion Enricher
}
