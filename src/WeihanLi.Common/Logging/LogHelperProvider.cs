namespace WeihanLi.Common.Logging
{
    public interface ILogHelperProvider
    {
        void Log(LogHelperLoggingEvent loggingEvent);
    }

    internal class NullLogHelperProvider : ILogHelperProvider
    {
        public void Log(LogHelperLoggingEvent loggingEvent)
        {
        }
    }
}
