namespace QuizDesk.Application.DTOs.OAuth
{
    public class OAuthUserInfo
    {
        public string ProviderUserId { get; init; }
        public string Email { get; init; } 
        public string FullName { get; init; } 
        public string? AvatarUrl { get; init; }
        public bool VerifiedEmail { get; init; }
    }
}
