namespace RateLimitedNotificationService.Models
{
    public class RateLimitRule
    {
        public string NotificationType { get; set; }
        public int Limit { get; set; }
        public int PeriodSeconds { get; set; }
    }
}
