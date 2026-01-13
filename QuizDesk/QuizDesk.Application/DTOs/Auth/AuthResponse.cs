namespace QuizDesk.Application.DTOs.Auth
{
    public class AuthResponse
    {
        public UserDto User { get; set; } = null!;
        public TokenResponse Tokens { get; set; } = null!;
        public bool IsNewUser { get; set; }
    }
}
