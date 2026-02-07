using QuizDesk.Domain.Entities;
using QuizDesk.Domain.ValueObjects;

namespace QuizDesk.Domain.Interfaces
{
    public interface IOAuthRepository: IRepository<UserOAuthAccount>
    {
        Task<UserOAuthAccount?> GetByProviderUserIdAsync(string provider, string providerUserId,
        CancellationToken cancellationToken = default);
        Task<List<UserOAuthAccount>> GetUserAccountsAsync(int userId,
            CancellationToken cancellationToken = default);
        Task<UserOAuthAccount?> GetByProviderAndUserAsync(string provider, int userId,
            CancellationToken cancellationToken = default);
    }
}
