using QuizDesk.Domain.ValueObjects;

namespace QuizDesk.Application.Abstractions
{
    public interface IEmailService
    {
        Task SendVerificationEmail(Email email, string name);
        Task SendPasswordResetEmail(Email email, string token);
        Task SendInvitationEmail(Email email, string examName, string invitationToken);
    }
}
