using QuizDesk.Domain.Entities;
using QuizDesk.Domain.ValueObjects;

namespace QuizDesk.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<User?> GetWithOAuthAccountsAsync(int id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<User>> GetUsersByRoleAsync(string role, CancellationToken cancellationToken = default);
    }
}
