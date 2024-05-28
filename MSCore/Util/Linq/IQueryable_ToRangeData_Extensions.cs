using System.Collections.Generic;
using System.Linq;


namespace MSCore.Util.Linq
{

    public static partial class IQueryable_ToRangeData_Extensions
    {

        public static RangeData<T> IQueryable_ToRangeData<T>(this IQueryable query, RangeInfo range)
        {
            if (query == null) return null;

            return new RangeData<T>(range) { totalCount = query.IQueryable_Count(), items = query.IQueryable_Range(range).IQueryable_ToList<T>() };
        }

        public static RangeData<T> IQueryable_ToRangeData<T>(this IQueryable query, PageInfo page)
        {
            return IQueryable_ToRangeData<T>(query, page.ToRange());
        }


    }
}