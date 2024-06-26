﻿using WeihanLi.Common.Models;
using Xunit;

namespace WeihanLi.Common.Test.ModelsTest;

public class ResultTest
{
    [Fact]
    public void SuccessTest()
    {
        var result = Result.Success();
        Assert.Null(result.Msg);
        Assert.Equal(ResultStatus.Success, result.Status);
    }

    [Theory]
    [InlineData(ResultStatus.Unauthorized)]
    [InlineData(ResultStatus.Forbidden)]
    [InlineData(ResultStatus.BadRequest)]
    [InlineData(ResultStatus.NotImplemented)]
    [InlineData(ResultStatus.NotFound)]
    [InlineData(ResultStatus.RequestTimeout)]
    public void FailTest(ResultStatus resultStatus)
    {
        var result = Result.Fail("test error", resultStatus);
        Assert.Equal(resultStatus, result.Status);
    }
}
