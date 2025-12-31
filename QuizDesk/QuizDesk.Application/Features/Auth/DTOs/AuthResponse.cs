using QuizDesk.Application.DTOs;

namespace QuizDesk.Application.Features.Auth.DTOs
{
    public class AuthResponse
    {
        public UserDto User { get; set; } = null!;
        public TokenResponse Tokens { get; set; } = null!;
        public bool IsNewUser { get; set; }
    }
}
