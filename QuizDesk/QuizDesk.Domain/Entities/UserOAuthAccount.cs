using QuizDesk.Domain.ValueObjects;

namespace QuizDesk.Domain.Entities
{
    public class UserOAuthAccount : BaseEntity
    {
        public int UserId { get; private set; }
        public string Provider { get; private set; }
        public string ProviderUserId { get; private set; }
        public Email Email { get; private set; }
        public string? AccessToken { get; private set; }
        public string? RefreshToken { get; private set; }
        public DateTime? TokenExpiresAt { get; private set; }
        public string? ProfileData { get; private set; }
        public bool? IsPrimary { get; private set; }

        public virtual User User { get; private set; }
        
        private UserOAuthAccount() { }
        public static UserOAuthAccount Create(int userId, string provider, string providerUserId, Email email, string? accessToken = null, string? refreshToken = null, DateTime? tokenExpiresAt = null, string? profileData = null)
        {
            return new UserOAuthAccount
            {
                UserId = userId,
                Provider = provider,
                ProviderUserId = providerUserId,
                Email = email,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenExpiresAt = tokenExpiresAt,
                ProfileData = profileData,
                IsPrimary = true,
            };
        }

        public void UpdateTokens(
            string accessToken,
            string? refreshToken = null,
            DateTime? tokenExpiresAt = null)
        {
            AccessToken = accessToken;
            if (refreshToken != null) RefreshToken = refreshToken;
            TokenExpiresAt = tokenExpiresAt;
            MarkUpdated();
        }

        public void SetAsPrimary(bool isPrimary)
        {
            IsPrimary = isPrimary;
            MarkUpdated();
        }
    }
}
