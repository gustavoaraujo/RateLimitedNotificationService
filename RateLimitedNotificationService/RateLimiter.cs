using Enyim.Caching.Memcached;
using RateLimitedNotificationService.Cache;
using RateLimitedNotificationService.Repositories;

namespace RateLimitedNotificationService
{
    public class RateLimiter
    {
        private readonly IMemcachedClientWrapper _client;

        private readonly RateLimitRepository _rateLimitRepository;

        public RateLimiter(IMemcachedClientWrapper client, RateLimitRepository rateLimitRepository)
        {
            _client = client;
            _rateLimitRepository = rateLimitRepository;
        }

        public virtual async Task<bool> CanSendAsync(string userId, string notificationType)
        {
            var rule = await _rateLimitRepository.GetRateLimitRuleAsync(notificationType);
            if (rule == null)
                throw new ArgumentException("Invalid notification type");

            var key = $"{userId}:{notificationType}";
            var currentCount = _client.Get<int?>(key) ?? 0;

            if (currentCount < rule.Limit)
            {
                currentCount++;
                _client.Store(StoreMode.Set, key, currentCount, TimeSpan.FromSeconds(rule.PeriodSeconds));
                return true;
            }

            return false;
        }
    }
}