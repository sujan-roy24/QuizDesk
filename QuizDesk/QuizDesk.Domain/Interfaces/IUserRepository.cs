using QuizDesk.Domain.Entities;
using QuizDesk.Domain.Enums;
using QuizDesk.Domain.ValueObjects;

namespace QuizDesk.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<User>> GetUsersByRoleAsync(UserRole role, CancellationToken cancellationToken = default);
    }
}
