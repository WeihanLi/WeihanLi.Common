using System.Linq;
using WeihanLi.Common.Models;
using WeihanLi.Extensions;
using Xunit;

namespace WeihanLi.Common.Test.ModelsTest
{
    public class PagedListModelTest
    {
        [Fact]
        public void PagedModelToJsonTest()
        {
            var model = new PagedListResult<int>()
            {
                PageNumber = 1,
                PageSize = 3,
                TotalCount = 10,
                Data = new[] { 1, 2, 3 }
            };
            var json = model.ToJson();
            var dModel = json.JsonToObject<PagedListResult<int>>();
            Assert.Equal(model.PageSize, dModel.PageSize);
        }

        [Fact]
        public void EmptyPagedListResultTest()
        {
            var empty = PagedListResult<int>.Empty;
            Assert.Empty(empty.Data);
            Assert.Equal(0, empty.TotalCount);
            Assert.Equal(1, empty.PageNumber);
            Assert.Equal(10, empty.PageSize);
            Assert.Equal(0, empty.PageCount);
        }

        [Fact]
        public void ListDataWithTotalToJsonTest()
        {
            var model = new ListResultWithTotal<int>()
            {
                TotalCount = 10,
                Data = new[] { 1, 2, 3 }
            };
            var json = model.ToJson();
            var dModel = json.JsonToObject<ListResultWithTotal<int>>();
            Assert.Equal(model.TotalCount, dModel.TotalCount);
            Assert.True(dModel.Data.SequenceEqual(model.Data));
        }

        [Fact]
        public void ListDataWithTotalEmptyTest()
        {
            var empty = ListResultWithTotal<int>.Empty;
            Assert.NotNull(empty.Data);
            Assert.Empty(empty.Data);
            Assert.Equal(0, empty.TotalCount);
        }
    }
}
