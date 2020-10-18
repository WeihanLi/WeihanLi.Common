namespace WeihanLi.Common.Models
{
    public class PagedRequest
    {
        private int _pageNum = 1;
        private int _pageSize = 10;

        /// <summary>
        /// PageNum
        /// 1 by default, 1 based
        /// </summary>
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

        /// <summary>
        /// PageSize
        /// 10 by default
        /// </summary>
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
