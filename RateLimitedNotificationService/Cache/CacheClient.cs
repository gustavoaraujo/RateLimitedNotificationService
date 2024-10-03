using Enyim.Caching;

namespace RateLimitedNotificationService.Cache
{
    public class CacheClient : MemcachedClient
    {
        public virtual int GetCount(string key)
        {
            return Get<int?>(key) ?? 0;
        }
    }
}
