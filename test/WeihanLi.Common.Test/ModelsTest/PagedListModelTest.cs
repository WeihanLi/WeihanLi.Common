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
            var model = new PagedListModel<int>()
            {
                PageNumber = 1,
                PageSize = 3,
                TotalCount = 10,
                Data = new[] { 1, 2, 3 }
            };
            var json = model.ToJson();
            var dModel = json.JsonToType<PagedListModel<int>>();
            Assert.Equal(model.PageSize, dModel.PageSize);
        }
    }
}
