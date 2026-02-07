using Microsoft.EntityFrameworkCore;
using QuizDesk.Domain.Entities;
using QuizDesk.Domain.Interfaces;
using QuizDesk.Domain.ValueObjects;

namespace QuizDesk.Infrastructure.Persistance.Repositories
{
    public class OAuthRepository : IOAuthRepository
    {
        private readonly AppDbContext _context;

        public OAuthRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(UserOAuthAccount entity, CancellationToken cancellationToken = default)
        {
            await _context.UserOAuthAccounts.AddAsync(entity, cancellationToken);
        }

        public Task DeleteAsync(UserOAuthAccount entity, CancellationToken cancellationToken = default)
        {
            _context.UserOAuthAccounts.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<IReadOnlyList<UserOAuthAccount>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.UserOAuthAccounts
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<UserOAuthAccount?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.UserOAuthAccounts
               .AsNoTracking()
               .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<UserOAuthAccount?> GetByProviderAndUserAsync(string provider, int userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserOAuthAccounts
                .FirstOrDefaultAsync(x => x.Provider == provider && x.UserId == userId, cancellationToken);
        }

        public async Task<UserOAuthAccount?> GetByProviderUserIdAsync(string provider, string providerUserId, CancellationToken cancellationToken = default)
        {
            return await _context.UserOAuthAccounts
                .FirstOrDefaultAsync(x => x.Provider == provider && x.ProviderUserId == providerUserId, cancellationToken);
        }

        public async Task<List<UserOAuthAccount>> GetUserAccountsAsync(int userId, CancellationToken cancellationToken = default)
        {
            return await _context.UserOAuthAccounts
                .Where(x => x.UserId == userId)
                .ToListAsync(cancellationToken);
        }
        public Task UpdateAsync(UserOAuthAccount entity, CancellationToken cancellationToken = default)
        {
            _context.UserOAuthAccounts.Update(entity);
            return Task.CompletedTask;
        }
    }
}
