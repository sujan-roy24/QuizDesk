using QuizDesk.Application.DTOs.Auth;

namespace QuizDesk.Application.Interfaces.Auth
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<AuthResponse> RefreshTokenAsync(string refreshToken);
        Task<UserDto> GetProfileAsync(int id);
        Task<bool> LogoutAsync(string refreshToken);
    }
}
