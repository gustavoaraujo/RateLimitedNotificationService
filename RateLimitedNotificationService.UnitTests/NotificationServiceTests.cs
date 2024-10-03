using Amazon.DynamoDBv2;
using Moq;
using RateLimitedNotificationService.Cache;
using RateLimitedNotificationService.Repositories;
using RateLimitedNotificationService.Service;

namespace RateLimitedNotificationService.UnitTests
{
    public class NotificationServiceTests
    {
        private readonly Mock<Gateway> _mockGateway;
        private readonly Mock<RateLimiter> _mockRateLimiter;
        private readonly NotificationService _notificationService;

        public NotificationServiceTests()
        {
            _mockGateway = new Mock<Gateway>();

            var mockClient = new Mock<IMemcachedClientWrapper>();
            var dynamoDb = new Mock<IAmazonDynamoDB>();
            var mockRepository = new Mock<RateLimitRepository>(dynamoDb.Object);

            _mockRateLimiter = new Mock<RateLimiter>(mockClient.Object, mockRepository.Object);
            _notificationService = new NotificationService(_mockGateway.Object, _mockRateLimiter.Object);
        }

        [Fact]
        public async Task Send_ShouldSendMessage_WhenRateLimitNotExceeded()
        {
            // Arrange
            string notificationType = "status";
            string userId = "user1";
            string message = "This is a test message.";

            _mockRateLimiter.Setup(r => r.CanSendAsync(userId, notificationType)).ReturnsAsync(true);

            // Act
            await _notificationService.Send(notificationType, userId, message);

            // Assert
            _mockGateway.Verify(g => g.Send(userId, message), Times.Once);
        }

        [Fact]
        public async Task Send_ShouldThrowException_WhenRateLimitExceeded()
        {
            // Arrange
            string notificationType = "status";
            string userId = "user1";
            string message = "This is a test message.";

            _mockRateLimiter.Setup(r => r.CanSendAsync(userId, notificationType)).ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _notificationService.Send(notificationType, userId, message));
            Assert.Equal("Rate limit exceeded for this notification type.", exception.Message);
        }
    }
}
