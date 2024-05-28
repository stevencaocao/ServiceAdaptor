using System.Collections.Generic;
using System.Linq;
namespace MSCore.Util.Linq
{

    public static partial class Queryable_ToPageData_Extensions
    {

        public static PageData<T> ToPageData<T>(this IQueryable<T> query, PageInfo page)
        {
            if (query == null) return null;

            return new PageData<T>(page) { totalCount = query.Count(), items = query.Page(page).ToList() };
        }

    }
}