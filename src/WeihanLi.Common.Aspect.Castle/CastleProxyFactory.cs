using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Aspect.Castle
{
    internal class CastleProxyFactory : IProxyFactory
    {
        public TService CreateProxy<TService>() where TService : class
        {
            var serviceType = typeof(TService);
            if (serviceType.IsInterface)
            {
                return CastleHelper.ProxyGenerator.CreateInterfaceProxyWithoutTarget<TService>();
            }
            return CastleHelper.ProxyGenerator.CreateClassProxy<TService>();
        }

        public TService CreateProxy<TService, TImplement>() where TService : class where TImplement : TService
        {
            var serviceType = typeof(TService);
            if (serviceType.IsInterface)
            {
                return CastleHelper.ProxyGenerator.CreateInterfaceProxyWithTarget<TService>(ActivatorHelper.CreateInstance<TImplement>());
            }
            return CastleHelper.ProxyGenerator.CreateClassProxyWithTarget<TService>(ActivatorHelper.CreateInstance<TImplement>());
        }

        public TService CreateProxy<TService, TImplement>(object[] parameters) where TService : class where TImplement : TService
        {
            var serviceType = typeof(TService);
            if (serviceType.IsInterface)
            {
                return CastleHelper.ProxyGenerator.CreateInterfaceProxyWithTarget<TService>(ActivatorHelper.CreateInstance<TImplement>(parameters));
            }
            return CastleHelper.ProxyGenerator.CreateClassProxyWithTarget<TService>(ActivatorHelper.CreateInstance<TImplement>(parameters));
        }
    }
}
