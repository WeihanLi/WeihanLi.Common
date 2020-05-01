using System;

namespace WeihanLi.Common.Aspect
{
    public class FluentAspects
    {
        public static readonly FluentAspectOptions AspectOptions;

        static FluentAspects()
        {
            AspectOptions = new FluentAspectOptions();
            // register built-in necessary interceptors
            AspectOptions.InterceptAll()
                .With<TryInvokeInterceptor>();
            AspectOptions.InterceptMethod<IDisposable>(m => m.Dispose())
                .With<DisposableInterceptor>();
        }

        public static void Configure(Action<FluentAspectOptions> optionsAction)
        {
            optionsAction?.Invoke(AspectOptions);
        }

        public static TService CreateProxy<TService>() where TService : class
        {
            return AspectOptions.ProxyFactory.CreateProxy<TService>();
        }

        public static TService CreateProxy<TService, TImplement>() where TService : class where TImplement : TService
        {
            return AspectOptions.ProxyFactory.CreateProxy<TService, TImplement>();
        }
    }
}
