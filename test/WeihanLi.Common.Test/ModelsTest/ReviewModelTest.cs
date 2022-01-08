using WeihanLi.Common.Models;
using Xunit;

namespace WeihanLi.Common.Test.ModelsTest;

public class ReviewModelTest
{
    [Fact]
    public void ReviewRequestTest()
    {
        var request = new ReviewRequest()
        {
            State = ReviewState.Reviewed
        };
        Assert.True(request.IsValid());

        request = new ReviewRequest()
        {
            State = ReviewState.UnReviewed,
        };
        Assert.True(request.IsValid());

        request = new ReviewRequest()
        {
            State = ReviewState.Rejected,
        };
        Assert.False(request.IsValid());

        request = new ReviewRequest()
        {
            State = ReviewState.Rejected,
            Remark = ""
        };
        Assert.False(request.IsValid());

        request = new ReviewRequest()
        {
            State = ReviewState.Rejected,
            Remark = " "
        };
        Assert.False(request.IsValid());

        request = new ReviewRequest()
        {
            State = ReviewState.Rejected,
            Remark = "just for test"
        };
        Assert.True(request.IsValid());
    }
}
