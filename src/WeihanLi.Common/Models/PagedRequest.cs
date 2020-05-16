namespace WeihanLi.Common.Models
{
    public class PagedRequest
    {
        private int _pageNum = 1;
        private int _pageSize = 10;

        public int PageNum
        {
            get => _pageNum;
            set => _pageNum = value > 0 ? value : 1;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > 0 ? value : 10;
        }
    }
}
