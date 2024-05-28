using System.Linq;

namespace MSCore.Util.Linq
{

    public static partial class IQueryable_Range_Extensions
    {
        public static IQueryable IQueryable_Range(this IQueryable query, RangeInfo range)
        {
            if (query == null || range == null) return query;

            return query.IQueryable_Range(range.skip, range.take);
        }



        public static IQueryable IQueryable_Range(this IQueryable query, int skip, int take)
        {
            return query?.IQueryable_Skip(skip).IQueryable_Take(take);
        }
    }
}