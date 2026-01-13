using QuizDesk.Application.Abstractions;
using QuizDesk.Domain.ValueObjects;
using BC = BCrypt.Net.BCrypt;

namespace QuizDesk.Infrastructure.Services.Security
{
    public class PasswordHasher : IPasswordHasher 
    { 
        private const int WorkFactor = 12; 
        public Password HashPassword(Password password) 
        { 
            var hash = BC.HashPassword(password.Hash, WorkFactor); 
            return Password.Create(hash); 
        } 
        public bool VerifyPassword(Password password, Password hash) 
        { 
            return BC.Verify(password.Hash, hash.Hash); 
        }
    }
}
