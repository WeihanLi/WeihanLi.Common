namespace WeihanLi.Common.Logging
{
    public interface ILogHelperProvider
    {
        ILogHelperLogger CreateLogger(string categoryName);
    }

    internal class NullLogHelperProvider : ILogHelperProvider
    {
        public ILogHelperLogger CreateLogger(string categoryName) => NullLogHelperLogger.Instance;
    }
}
