using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using RateLimitedNotificationService.Models;

namespace RateLimitedNotificationService.Repositories
{
    public class RateLimitRepository
    {
        private readonly DynamoDBContext _context;

        public RateLimitRepository(IAmazonDynamoDB dynamoDb)
        {
            _context = new DynamoDBContext(dynamoDb);
        }

        public virtual async Task<RateLimitRule> GetRateLimitRuleAsync(string notificationType)
        {
            return await _context.LoadAsync<RateLimitRule>(notificationType);
        }
    }
}
