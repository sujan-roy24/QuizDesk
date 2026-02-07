namespace QuizDesk.Application.DTOs.OAuth
{
    public class OAuthRequest
    {
        public string Code { get; set; } = string.Empty;
        public string? State { get; set; }
    }
}
