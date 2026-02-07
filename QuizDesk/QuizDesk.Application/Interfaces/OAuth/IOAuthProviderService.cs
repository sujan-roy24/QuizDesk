using QuizDesk.Application.DTOs.OAuth;
namespace QuizDesk.Application.Interfaces.OAuth
{
    public interface IOAuthProviderService 
    { 
        string ProviderKey { get; } 
        Task<OAuthTokens> ExchangeCodeAsync(string code); 
        Task<OAuthUserInfo> GetUserInfoAsync(string accessToken); 
    }
 
}
