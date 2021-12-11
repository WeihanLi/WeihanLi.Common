using WeihanLi.Common.Models;
using Xunit;

namespace WeihanLi.Common.Test.ModelsTest;

public class PagedRequestTest
{
    [Theory]
    [InlineData(1, 1)]
    [InlineData(1, 10)]
    [InlineData(100, 10)]
    public void ValidPageRequestTest(int pageNum, int pageSize)
    {
        var pagedRequest = new PagedRequest()
        {
            PageNum = pageNum,
            PageSize = pageSize,
        };
        Assert.Equal(pageNum, pagedRequest.PageNum);
        Assert.Equal(pageSize, pagedRequest.PageSize);
    }

    [Theory]
    [InlineData(-1, -1)]
    [InlineData(-1, -10)]
    [InlineData(0, -1)]
    public void InvalidPageRequestTest(int pageNum, int pageSize)
    {
        var pagedRequest = new PagedRequest()
        {
            PageNum = pageNum,
            PageSize = pageSize,
        };
        Assert.Equal(1, pagedRequest.PageNum);
        Assert.Equal(10, pagedRequest.PageSize);
    }
}
