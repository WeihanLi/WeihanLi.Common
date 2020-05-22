namespace WeihanLi.Common.Models
{
    public class PagedRequest
    {
        private int _pageNum = 1;
        private int _pageSize = 10;

        public int PageNum
        {
            get => _pageNum;
            set
            {
                if (value > 0)
                {
                    _pageNum = value;
                }
            }
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value > 0)
                {
                    _pageSize = value;
                }
            }
        }
    }
}
