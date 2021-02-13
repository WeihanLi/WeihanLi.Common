using System.Threading;
using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test.ExtensionsTest
{
    public class TaskExtensionsTest
    {
        [Fact]
        public void CancellationTokenAsTask()
        {
            using var cts = new CancellationTokenSource();
            var cancellationToken = cts.Token;

            var task = cancellationToken.AsTask();
            Assert.False(task.IsCompleted);
            Assert.False(cancellationToken.IsCancellationRequested);

            cts.Cancel();
            Assert.True(task.IsCompleted);
            Assert.True(cancellationToken.IsCancellationRequested);
        }
    }
}
