﻿namespace RateLimitedNotificationService.Service
{
    public class ConfigurationData
    {
        public string CacheEndpoint { get; set; }
        public int CachePort { get; set; }
        public string DbServiceURL { get; set; }
    }
}
