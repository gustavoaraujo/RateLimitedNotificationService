using Amazon.DynamoDBv2;
using Enyim.Caching.Memcached;
using Moq;
using RateLimitedNotificationService.Cache;
using RateLimitedNotificationService.Models;
using RateLimitedNotificationService.Repositories;

namespace RateLimitedNotificationService.UnitTests
{
    public class RateLimiterTests
    {
        private readonly Mock<IMemcachedClientWrapper> _mockClient;
        private readonly Mock<RateLimitRepository> _mockRepository;
        private readonly RateLimiter _rateLimiter;

        public RateLimiterTests()
        {
            _mockClient = new Mock<IMemcachedClientWrapper>();
            var dynamoDb = new Mock<IAmazonDynamoDB>();
            _mockRepository = new Mock<RateLimitRepository>(dynamoDb.Object);
            _rateLimiter = new RateLimiter(_mockClient.Object, _mockRepository.Object);
        }

        [Fact]
        public async Task CanSendAsync_ShouldReturnTrue_WhenBelowLimit()
        {
            // Arrange
            string userId = "user1";
            string notificationType = "status";

            _mockRepository.Setup(repo => repo.GetRateLimitRuleAsync(notificationType))
                           .ReturnsAsync(new RateLimitRule { Limit = 2, PeriodSeconds = 60 });

            _mockClient.Setup(client => client.Get<int?>($"{userId}:{notificationType}")).Returns(1);

            // Act
            var result = await _rateLimiter.CanSendAsync(userId, notificationType);

            // Assert
            Assert.True(result);
            _mockClient.Verify(client => client.Store(StoreMode.Set, $"{userId}:{notificationType}", 2, TimeSpan.FromSeconds(60)), Times.Once);
        }

        [Fact]
        public async Task CanSendAsync_ShouldReturnFalse_WhenLimitExceeded()
        {
            // Arrange
            string userId = "user1";
            string notificationType = "status";

            _mockRepository.Setup(repo => repo.GetRateLimitRuleAsync(notificationType))
                           .ReturnsAsync(new RateLimitRule { Limit = 2, PeriodSeconds = 60 });

            _mockClient.Setup(client => client.Get<int?>($"{userId}:{notificationType}")).Returns(2);

            // Act
            var result = await _rateLimiter.CanSendAsync(userId, notificationType);

            // Assert
            Assert.False(result);
            _mockClient.Verify(client => client.Store(It.IsAny<StoreMode>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<TimeSpan>()), Times.Never);
        }

        [Fact]
        public async Task CanSendAsync_ShouldThrowException_WhenInvalidNotificationType()
        {
            // Arrange
            string userId = "user1";
            string notificationType = "invalidType";

            _mockRepository.Setup(repo => repo.GetRateLimitRuleAsync(notificationType))
                           .ReturnsAsync((RateLimitRule)null); // Simula uma notificação inválida

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _rateLimiter.CanSendAsync(userId, notificationType));
        }

    }
}
