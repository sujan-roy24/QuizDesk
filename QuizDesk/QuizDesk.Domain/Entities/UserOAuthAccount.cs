using QuizDesk.Domain.ValueObjects;

namespace QuizDesk.Domain.Entities
{
    public class UserOAuthAccount : BaseEntity
    {
        public int UserId { get; private set; }
        public string Provider { get; private set; }
        public string ProviderUserId { get; private set; }
        public Email Email { get; private set; }
        private UserOAuthAccount() { }
        public static UserOAuthAccount Create(int userId, string provider, string providerUserId, Email email)
        {
            return new UserOAuthAccount
            {
                UserId = userId,
                Provider = provider,
                ProviderUserId = providerUserId,
                Email = email
            };
        }
    }
}
