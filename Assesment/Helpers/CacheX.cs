using AssesmentProject.Controllers;
using Microsoft.Extensions.Caching.Memory;

namespace Assesment.Helpers
{
    public class CacheX
    {
        public class CacheKey
        {
            public const string cache_key_SelectUserList = "cacheSelectUserList";
            public const string cache_key_SelectUserById = "cacheSelectUserById";
        }


        public static void AddCache(TimeSpan sec, IEnumerable<dynamic>ls, string cache_key, IMemoryCache _cache)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(sec)
                .SetPriority(CacheItemPriority.Normal);
            _cache.Set(cache_key, ls, cacheEntryOptions);
        }

    }
}
