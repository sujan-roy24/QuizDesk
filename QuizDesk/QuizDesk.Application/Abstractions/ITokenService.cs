using QuizDesk.Application.Contracts.Auth;
using QuizDesk.Domain.Entities;

namespace QuizDesk.Application.Abstractions
{
    public interface ITokenService
    {
        Task<TokenResponse> GenerateTokens(User user);
        Task<TokenResponse> RefreshToken(string refreshToken);
        Task<bool> RevokeToken(string refreshToken);
        Task<bool> ValidateToken(string token);
    }
}
