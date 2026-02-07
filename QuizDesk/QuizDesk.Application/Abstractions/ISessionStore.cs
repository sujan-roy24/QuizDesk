using QuizDesk.Application.DTOs.Models;

namespace QuizDesk.Application.Abstractions
{
    public interface ISessionStore
    {
        Task SaveAsync(SessionData session);
        Task<SessionData?> GetAsync(string provider, int userId);
        Task RemoveAsync(string provider, int userId);
    }
}
