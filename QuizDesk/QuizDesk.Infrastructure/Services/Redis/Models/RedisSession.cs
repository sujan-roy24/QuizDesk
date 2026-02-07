namespace QuizDesk.Infrastructure.Services.Redis.Models
{
    public sealed class RedisSession
    {
        public int UserId { get; init; }
        public string Provider { get; init; }
        public string EncryptedAccessToken { get; init; }
        public string? EncryptedRefreshToken { get; init; }
        public DateTime ExpiresAt { get; init; }
    }
}
