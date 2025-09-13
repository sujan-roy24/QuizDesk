using QuizDesk.Application.Common;
using QuizDesk.Application.DTOs;

namespace QuizDesk.Application.Interfaces
{
    public interface IAuthService
    {
        Task<Result<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
        Task<Result<AuthResponseDto>> LoginAsync(LoginRequestDto request);
        Task<Result> LogoutAsync();
    }
}
