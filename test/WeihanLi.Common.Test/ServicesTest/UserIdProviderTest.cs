using System;
using WeihanLi.Common.Services;
using Xunit;

namespace WeihanLi.Common.Test.ServicesTest
{
    public class UserIdProviderTest
    {
        [Fact]
        public void EnvironmentUserIdProviderTest()
        {
            IUserIdProvider userIdProvider = new EnvironmentUserIdProvider();
            var userId = userIdProvider.GetUserId();
            Assert.Equal(Environment.UserName, userId);
        }
    }
}
