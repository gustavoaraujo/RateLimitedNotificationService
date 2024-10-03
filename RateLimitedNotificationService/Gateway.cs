namespace RateLimitedNotificationService
{
    public class Gateway
    {
        public virtual void Send(string userId, string message)
        {
            Console.WriteLine($"Sending message to user {userId}: {message}");
        }
    }
}