using WeihanLi.Common.Models;
using Xunit;

namespace WeihanLi.Common.Test.ModelsTest
{
    public class ResultModelTest
    {
        [Fact]
        public void SuccessTest()
        {
            var result = ResultModel.Success();
            Assert.Null(result.ErrorMsg);
            Assert.Equal(ResultStatus.Success, result.Status);
        }

        [Theory]
        [InlineData(ResultStatus.Unauthorized)]
        [InlineData(ResultStatus.NoPermission)]
        [InlineData(ResultStatus.RequestError)]
        [InlineData(ResultStatus.NotImplemented)]
        [InlineData(ResultStatus.ResourceNotFound)]
        [InlineData(ResultStatus.RequestTimeout)]
        public void FailTest(ResultStatus resultStatus)
        {
            var result = ResultModel.Fail("test error", resultStatus);
            Assert.Equal(resultStatus, result.Status);
        }
    }
}
