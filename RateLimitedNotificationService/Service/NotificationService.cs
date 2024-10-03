using Amazon.DynamoDBv2;
using Amazon.ElastiCacheCluster;
using Enyim.Caching;
using Newtonsoft.Json;
using RateLimitedNotificationService.Cache;
using RateLimitedNotificationService.Config;
using RateLimitedNotificationService.Repositories;

namespace RateLimitedNotificationService.Service
{
    public class NotificationService
    {
        private readonly Gateway _gateway;
        private readonly RateLimiter _rateLimiter;
        
        public NotificationService(Gateway gateway, RateLimiter rateLimiter)
        {
            _gateway = gateway;
            _rateLimiter = rateLimiter;
        }

        public NotificationService(Gateway gateway)
        {
            _gateway = gateway;
            ConfigurationData configurationData;

            using (StreamReader r = new StreamReader("Config/appconfig.json"))
            {
                string json = r.ReadToEnd();
                configurationData = JsonConvert.DeserializeObject<ConfigurationData>(json);
            }

            var config = new ElastiCacheClusterConfig(configurationData.CacheEndpoint, configurationData.CachePort);
            var client = new MemcachedClient(config);
            var clientWrapper = new MemcachedClientWrapper(client);

            var clientConfig = new AmazonDynamoDBConfig
            {
                ServiceURL = configurationData.DbServiceURL
            };

            var rateLimitRepository = new RateLimitRepository(new AmazonDynamoDBClient(clientConfig));
            _rateLimiter = new RateLimiter(clientWrapper, rateLimitRepository);

        }

        public async Task Send(string type, string userId, string message)
        {
            if (await _rateLimiter.CanSendAsync(userId, type))
            {
                _gateway.Send(userId, message);
            }
            else
            {
                throw new InvalidOperationException("Rate limit exceeded for this notification type.");
            }
        }
    }
}
