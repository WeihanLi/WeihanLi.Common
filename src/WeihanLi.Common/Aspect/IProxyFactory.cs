namespace WeihanLi.Common.Aspect
{
    public interface IProxyFactory
    {
        TService CreateProxy<TService>() where TService : class;

        TService CreateProxy<TService, TImplement>() where TImplement : TService where TService : class;

        TService CreateProxy<TService, TImplement>(object[] parameters) where TImplement : TService where TService : class;
    }
}
