namespace QuizDesk.Application.DTOs.OAuth
{
    public class OAuthTokens
    {
        public string AccessToken { get; init; } = string.Empty;
        public string? RefreshToken { get; init; }
        public int ExpiresIn { get; init; }
        public DateTime ExpiresAt => DateTime.UtcNow.AddSeconds(ExpiresIn);
    }
}
