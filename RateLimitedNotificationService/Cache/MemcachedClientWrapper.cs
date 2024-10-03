using Enyim.Caching.Memcached;
using Enyim.Caching;

namespace RateLimitedNotificationService.Cache
{
    public interface IMemcachedClientWrapper
    {
        T Get<T>(string key);
        bool Store(StoreMode mode, string key, object value, TimeSpan validFor);
    }

    public class MemcachedClientWrapper : IMemcachedClientWrapper
    {
        private readonly MemcachedClient _client;

        public MemcachedClientWrapper(MemcachedClient client)
        {
            _client = client;
        }

        public T Get<T>(string key)
        {
            return _client.Get<T>(key);
        }

        public bool Store(StoreMode mode, string key, object value, TimeSpan validFor)
        {
            return _client.Store(mode, key, value, validFor);
        }
    }

}
