using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace MSCore.Util.MemoryCache
{
    public class MsCache: Microsoft.Extensions.Caching.Memory.MemoryCache
    {
        public MsCache():this(new MemoryCacheOptions())
        {
        }

        public MsCache(IOptions<MemoryCacheOptions> optionsAccessor) : base(optionsAccessor)
        {
        }

        public static readonly MsCache Instance = new MsCache();
    }
}
