using System.Linq;

namespace MSCore.Util.Linq
{

    public static partial class Queryable_Page_Extensions
    {

        public static IQueryable<T> Page<T>(this IQueryable<T> query, PageInfo page)
        {
            return query.Range(page.ToRange());
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"> the index of the page (starting from 1) </param>
        /// <returns></returns>
        public static IQueryable<T> Page<T>(this IQueryable<T> query, int pageSize, int pageIndex = 1)
        {
            return query.Page(new PageInfo(pageSize: pageSize, pageIndex: pageIndex));
        }
    }
}