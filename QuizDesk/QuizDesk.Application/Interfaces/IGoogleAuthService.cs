using QuizDesk.Application.Common;
using QuizDesk.Application.DTOs;

namespace QuizDesk.Application.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<Result<AuthResponseDto>> AuthenticateAsync(string idToken);
    }
}
