using WeihanLi.Common.Services;
using Xunit;

namespace WeihanLi.Common.Test.ServicesTest;

public class UserIdProviderTest
{
    [Fact]
    public void EnvironmentUserIdProviderTest()
    {
        IUserIdProvider userIdProvider = EnvironmentUserIdProvider.Instance.Value;
        var userId = userIdProvider.GetUserId();
        Assert.Equal(Environment.UserName, userId);
        Assert.True(userIdProvider.TryGetUserId<string>(out var _));
        Assert.False(userIdProvider.TryGetUserId<int>(out var _));
        Assert.Equal(0, userIdProvider.GetUserId<int>());
    }
}
