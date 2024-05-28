using System.Collections.Generic;
using System.Linq;

namespace MSCore.Util.Linq
{

    public static partial class Queryable_ToRangeData_Extensions
    {

        public static RangeData<T> ToRangeData<T>(this IQueryable<T> query, RangeInfo range)
        {
            if (query == null) return null;

            return new RangeData<T>(range) { totalCount = query.Count(), items = query.Range(range).ToList() };
        }

        public static RangeData<T> ToRangeData<T>(this IQueryable<T> query, PageInfo page)
        {
            return ToRangeData(query, page.ToRange());
        }


    }
}