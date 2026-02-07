namespace QuizDesk.Application.DTOs.OAuth
{
    public class LinkedAccountDto
    {
        public string Provider { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public DateTime LinkedAt { get; set; }
    }
}
