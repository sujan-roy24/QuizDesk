using QuizDesk.Application.Contracts.OAuth;
using QuizDesk.Application.DTOs.Auth;
using QuizDesk.Application.DTOs.OAuth;

namespace QuizDesk.Application.Interfaces.OAuth
{
    public interface IOAuthService
    {
        Task<AuthResponse> LoginAsync(OAuthRequest request);
        //Task LinkAccountAsync(int userId, OAuthRequest request);
        //Task UnlinkAccountAsync(int userId);
        //Task<List<LinkedAccountDto>> GetLinkedAccountsAsync(int userId);
    }

}
