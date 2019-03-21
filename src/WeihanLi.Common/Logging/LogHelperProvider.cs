namespace WeihanLi.Common.Logging
{
    public interface ILogHelperProvider
    {
        ILogHelperLogger CreateLogger(string categoryName);
    }

    public class NullLogHelperProvider : ILogHelperProvider
    {
        public ILogHelperLogger CreateLogger(string categoryName) => NullLogHelperLogger.Instance;
    }
}
