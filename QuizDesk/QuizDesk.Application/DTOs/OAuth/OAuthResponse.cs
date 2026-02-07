using QuizDesk.Application.DTOs.Auth;

namespace QuizDesk.Application.Contracts.OAuth
{
    public class OAuthResponse
    {
        public AuthResponse Auth { get; set; } = null!;
        public bool IsNewUser { get; set; }
        public bool RequiresPasswordSetup { get; set; }
    }
}
