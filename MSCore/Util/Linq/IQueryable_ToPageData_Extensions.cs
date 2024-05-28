using System.Collections.Generic;
using System.Linq;

namespace MSCore.Util.Linq
{

    public static partial class IQueryable_ToPageData_Extensions
    {

        public static PageData<T> IQueryable_ToPageData<T>(this IQueryable query, PageInfo page)
        {
            if (query == null) return null;

            return new PageData<T>(page) { totalCount = query.IQueryable_Count(), items = query.IQueryable_Page(page).IQueryable_ToList<T>() };
        }
    }
}