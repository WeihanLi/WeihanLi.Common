using System.Collections.Generic;
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
        public bool BatchSupported => false;

        public Task Log(LogHelperLoggingEvent loggingEvent) => TaskHelper.CompletedTask;

        public Task Log(IEnumerable<LogHelperLoggingEvent> loggingEvents) => TaskHelper.CompletedTask;
    }
}
