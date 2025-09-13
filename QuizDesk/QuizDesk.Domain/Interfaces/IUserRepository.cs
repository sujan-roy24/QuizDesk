﻿using QuizDesk.Domain.Entities;

namespace QuizDesk.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> EmailExistsAsync(string email);
        Task<User?> GetUserWithRolesAsync(Guid userId);
    }
}
