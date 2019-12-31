using System.Threading.Tasks;
using WeihanLi.Common.Helpers;

namespace WeihanLi.Common.Logging
{
    public interface ILogHelperProvider
    {
        Task Log(LogHelperLoggingEvent loggingEvent);
    }

    internal class NullLogHelperProvider : ILogHelperProvider
    {
        public Task Log(LogHelperLoggingEvent loggingEvent) => TaskHelper.CompletedTask;
    }
}
