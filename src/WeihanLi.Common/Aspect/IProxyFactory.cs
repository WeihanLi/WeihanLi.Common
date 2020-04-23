namespace WeihanLi.Common.Aspect
{
    public interface IProxyFactory
    {
        TService CreateProxy<TService>();

        TService CreateProxy<TService, TImplement>() where TImplement : TService;

        TService CreateProxy<TService, TImplement>(object[] parameters) where TImplement : TService;
    }
}
