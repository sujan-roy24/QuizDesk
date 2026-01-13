using Microsoft.EntityFrameworkCore;
using QuizDesk.Domain.Entities;
using QuizDesk.Domain.Enums;
using QuizDesk.Domain.Interfaces;
using QuizDesk.Domain.ValueObjects;

namespace QuizDesk.Infrastructure.Persistance.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            Console.WriteLine("UserRepository");
            _context = context;
        }
        public async Task AddAsync(User entity, CancellationToken cancellationToken = default)
        {
            await _context.Users.AddAsync(entity, cancellationToken);
        }

        public Task DeleteAsync(User entity, CancellationToken cancellationToken = default)
        {
            _context.Users.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task<bool> ExistsByEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AnyAsync(u => u.Email.Value == email.Value, cancellationToken);
        }

        public async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AsNoTracking()
                .ToListAsync(cancellationToken);
                
        }

        public async Task<User?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email.Value == email.Value, cancellationToken);
        }

        public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<User>> GetUsersByRoleAsync(UserRole role, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Role == role)
                .ToListAsync(cancellationToken);
        }

        public async Task<User?> GetWithOAuthAccountsAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.Users
             .Include(u => u.OAuthAccounts)
             .AsNoTracking()
             .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        }

        public Task UpdateAsync(User entity, CancellationToken cancellationToken = default)
        {
            _context.Users.Update(entity);
            return Task.CompletedTask;
        }
    }
}
