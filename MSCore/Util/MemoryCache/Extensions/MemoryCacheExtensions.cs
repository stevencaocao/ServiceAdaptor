using Microsoft.Extensions.Caching.Memory;
using MSCore.Util.Object.Object_Extensions;
using System;
using System.Runtime.CompilerServices;

namespace MSCore.Util.MemoryCache.Extensions
{
    public static partial class MemoryCacheExtensions
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static TItem SetWithSlidingExpiration<TItem>(this Microsoft.Extensions.Caching.Memory.MemoryCache memoryCache, object key, TItem value, TimeSpan offset)
        {
            using (ICacheEntry cacheEntry = memoryCache.CreateEntry(key))
            {
                cacheEntry.SetSlidingExpiration(offset);
                cacheEntry.Value = value;
            }
            return value;
        }


    }
}
