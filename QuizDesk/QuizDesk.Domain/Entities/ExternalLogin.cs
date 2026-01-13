namespace QuizDesk.Domain.Entities
{
    public class ExternalLogin
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string ProviderKey { get; set; } = string.Empty;
        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;
    }
}
