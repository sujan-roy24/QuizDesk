namespace QuizDesk.Application.DTOs.Models
{
    public class SessionData
    {
        public int UserId { get; init; }
        public string Provider { get; init; }
        public string AccessToken { get; init; }
        public string? RefreshToken { get; init; }
        public DateTime ExpiresAt { get; init; }
    }
}
