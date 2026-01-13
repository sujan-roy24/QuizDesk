using QuizDesk.Domain.ValueObjects;

namespace QuizDesk.Application.Abstractions
{
    public interface IPasswordHasher
    {
        Password HashPassword(Password password);
        bool VerifyPassword(Password password, Password hash);
    }
}
