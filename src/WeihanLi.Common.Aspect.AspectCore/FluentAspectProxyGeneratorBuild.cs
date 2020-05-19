using AspectCore.Configuration;
using AspectCore.DependencyInjection;
using AspectCore.DynamicProxy;
using System;

namespace WeihanLi.Common.Aspect.AspectCore
{
    internal sealed class FluentAspectProxyGeneratorBuild
    {
        private readonly IAspectConfiguration _configuration;
        private readonly IServiceContext _serviceContext;

        public FluentAspectProxyGeneratorBuild()
        {
            _configuration = new FluentAspectConfiguration()
                ;
            _serviceContext = new ServiceContext(_configuration);
        }

        public FluentAspectProxyGeneratorBuild Configure(Action<IAspectConfiguration> options = null)
        {
            options?.Invoke(_configuration);
            return this;
        }

        public FluentAspectProxyGeneratorBuild ConfigureService(
            Action<IServiceContext> options = null)
        {
            options?.Invoke(_serviceContext);
            return this;
        }

        public IProxyGenerator Build()
        {
            return (IProxyGenerator)new DisposedProxyGenerator(_serviceContext.Build());
        }
    }

    internal sealed class FluentAspectConfiguration : IAspectConfiguration
    {
        public AspectValidationHandlerCollection ValidationHandlers { get; }

        public InterceptorCollection Interceptors { get; }

        public NonAspectPredicateCollection NonAspectPredicates { get; }

        public bool ThrowAspectException { get; set; }

        public FluentAspectConfiguration()
        {
            ThrowAspectException = true;
            ValidationHandlers = new AspectValidationHandlerCollection();
            Interceptors = new InterceptorCollection();
            NonAspectPredicates = new NonAspectPredicateCollection();

            ValidationHandlers.Add(new OverwriteAspectValidationHandler());
            ValidationHandlers.Add(new AttributeAspectValidationHandler());
            ValidationHandlers.Add(new CacheAspectValidationHandler());
            ValidationHandlers.Add(new ConfigureAspectValidationHandler(this));
        }
    }

    internal sealed class DisposedProxyGenerator : IProxyGenerator, IDisposable
    {
        private readonly IServiceResolver _serviceResolver;
        private readonly IProxyGenerator _proxyGenerator;

        public DisposedProxyGenerator(IServiceResolver serviceResolver)
        {
            _serviceResolver = serviceResolver;
            _proxyGenerator = serviceResolver.ResolveRequired<IProxyGenerator>();
        }

        public IProxyTypeGenerator TypeGenerator
        {
            get
            {
                return _proxyGenerator.TypeGenerator;
            }
        }

        public object CreateClassProxy(Type serviceType, Type implementationType, object[] args)
        {
            return _proxyGenerator.CreateClassProxy(serviceType, implementationType, args);
        }

        public object CreateInterfaceProxy(Type serviceType)
        {
            return _proxyGenerator.CreateInterfaceProxy(serviceType);
        }

        public object CreateInterfaceProxy(Type serviceType, object implementationInstance)
        {
            return _proxyGenerator.CreateInterfaceProxy(serviceType, implementationInstance);
        }

        public void Dispose()
        {
            _serviceResolver.Dispose();
        }
    }
}
