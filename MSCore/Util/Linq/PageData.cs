using System.Collections.Generic;
namespace MSCore.Util.Linq
{
    public class PageData : PageData<object>
    {
        public PageData() { }
        public PageData(int pageSize, int pageIndex = 1) : base(pageSize, pageIndex) { }

        public PageData(PageInfo pageInfo = null) : base(pageInfo) { }
    }

    public class PageData<T> : PageInfo
    {

        public PageData() { }

        public PageData(int pageSize, int pageIndex = 1) : base(pageSize, pageIndex) { }


        public PageData(PageInfo pageInfo) : base(pageInfo.pageSize, pageInfo.pageIndex) { }


        /// <summary>
        /// data items for the current page
        /// </summary>
        public List<T> items { get; set; }


        /// <summary>
        /// total count of records
        /// </summary>
        public int totalCount { get; set; }
    }
}
