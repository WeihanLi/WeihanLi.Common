namespace WeihanLi.Common.Aspect
{
    public interface IProxyFactory
    {
        TService CreateProxy<TService>();

        TService CreateProxy<TService, TImplement>();

        TService CreateProxy<TService, TImplement>(object[] parameters);
    }
}
