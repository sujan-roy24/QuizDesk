using QuizDesk.Domain.Enums;
using QuizDesk.Domain.ValueObjects;

namespace QuizDesk.Domain.Entities
{
    public class User : BaseEntity
    {
        public Email Email { get; private set; }
        public Password? PasswordHash { get; private set; }
        public string FullName { get; private set; }
        public UserRole Role { get; private set; }
        public AuthMethod AuthMethod { get; private set; }
        public bool IsActive { get; private set; }
        public bool EmailVerified { get; private set; }
        public string? AvatarUrl { get; private set; }
        public DateTime? LastLogin { get; private set; }

        public virtual ICollection<UserOAuthAccount> OAuthAccounts { get; private set; } = new List<UserOAuthAccount>();

        private User() { }
        internal static User Create(Email email, string fullName, UserRole role, AuthMethod authMethod) 
        {
            return new User
            {
                Email = email,
                FullName = fullName,
                Role = role,
                AuthMethod = authMethod,
                IsActive = true,
                EmailVerified = authMethod == AuthMethod.Google, 
            };
        }

        public void UpdateProfile (string fullName, string? avatarUrl)
        {
            FullName = fullName;
            AvatarUrl = avatarUrl;
            MarkUpdated();
        }
        public void ChangeRole(UserRole newRole)
        {
            Role = newRole;
            MarkUpdated();
        }
        public void SetPassword(Password passwordHash)
        {
            PasswordHash = passwordHash;
            AuthMethod = AuthMethod == AuthMethod.Google ? AuthMethod.Both : AuthMethod.Password;
            MarkUpdated();
        }
        public void MarkEmailVerified()
        {
            EmailVerified = true;
            MarkUpdated();
        }
        public void UpdateLastLogin()
        {
            LastLogin = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            MarkUpdated();
        }
        public void Activate()
        {
            IsActive = true;
            MarkUpdated();
        }
    }
}
