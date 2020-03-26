using WeihanLi.Common.Models;
using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test.ModelsTest
{
    public class PagedListModelTest
    {
        [Fact]
        public void JsonTest()
        {
            var model = new PagedListData<int>()
            {
                PageNum = 1,
                PageSize = 3,
                TotalCount = 10,
                Data = new[] { 1, 2, 3 }
            };
            var json = model.ToJson();
            var dModel = json.JsonToObject<PagedListData<int>>();
            Assert.Equal(model.PageSize, dModel.PageSize);
        }

        [Fact]
        public void EmptyTest()
        {
            var empty = PagedListData<int>.Empty;
            Assert.Empty(empty.Data);
            Assert.Equal(0, empty.TotalCount);
            Assert.Equal(1, empty.PageNum);
            Assert.Equal(10, empty.PageSize);
            Assert.Equal(0, empty.PageCount);
        }
    }
}
