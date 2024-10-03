using RateLimitedNotificationService;
using RateLimitedNotificationService.Service;

var notificationService = new NotificationService(new Gateway());
await notificationService.Send("alert", "user 1", "Game over!");