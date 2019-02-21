using System;

namespace WeihanLi.Common.Logging
{
    public interface ILogHelperProvider
    {
        ILogHelperLogger CreateLogger(string categoryName);
    }

    public class NullLogHelperProvider : ILogHelperProvider
    {
        private static readonly Lazy<ILogHelperLogger> _loggerLazy = new Lazy<ILogHelperLogger>(() => new NullLogHelperLogger());

        public ILogHelperLogger CreateLogger(string categoryName)
        {
            return _loggerLazy.Value;
        }
    }
}
