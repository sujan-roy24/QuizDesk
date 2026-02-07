using Microsoft.Extensions.Configuration;
using QuizDesk.Application.DTOs.OAuth;
using QuizDesk.Application.Interfaces.OAuth;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace QuizDesk.Infrastructure.Services.OAuth
{
    public sealed class GoogleOAuthProviderService : IOAuthProviderService
    {
        public string ProviderKey => "google";

        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public GoogleOAuthProviderService( HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<OAuthTokens> ExchangeCodeAsync(string code)
        {
            var payload = new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] = _config["OAuth:Google:ClientId"],
                ["client_secret"] = _config["OAuth:Google:ClientSecret"],
                ["redirect_uri"] = _config["OAuth:Google:RedirectUri"]!,
                ["grant_type"] = "authorization_code"
            };

            var response = await _http.PostAsync(
                "https://oauth2.googleapis.com/token",
                new FormUrlEncodedContent(payload));

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new InvalidOperationException(
                    $"Google token exchange failed: {response.StatusCode} - {errorContent}");
            }

            var json = await response.Content.ReadAsStringAsync();
            var token = JsonSerializer.Deserialize<GoogleTokenResponse>(json)
                ?? throw new InvalidOperationException("Failed to deserialize token response"); ;

            return new OAuthTokens
            {
                AccessToken = token.AccessToken,
                RefreshToken = token.RefreshToken,
                ExpiresIn = token.ExpiresIn
            };
        }

        public async Task<OAuthUserInfo> GetUserInfoAsync(string accessToken)
        {
            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", accessToken);

            var response = await _http.GetAsync(
                "https://www.googleapis.com/oauth2/v2/userinfo");

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException("Google userinfo request failed");

            var json = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<GoogleUserInfoResponse>(json)!;

            return new OAuthUserInfo
            {
                ProviderUserId = user.Id,
                Email = user.Email,
                FullName = user.Name,
                VerifiedEmail = user.VerifiedEmail,
                AvatarUrl = user.Picture
            };
        }

        private sealed class GoogleTokenResponse
        {
            [JsonPropertyName("access_token")]
            public string AccessToken { get; set; } = string.Empty;

            [JsonPropertyName("expires_in")]
            public int ExpiresIn { get; set; }

            [JsonPropertyName("refresh_token")]
            public string? RefreshToken { get; set; }
        }

        private sealed class GoogleUserInfoResponse
        {
            [JsonPropertyName("id")]
            public string Id { get; set; } = string.Empty;

            [JsonPropertyName("email")]
            public string Email { get; set; } = string.Empty;

            [JsonPropertyName("verified_email")]
            public bool VerifiedEmail { get; set; }

            [JsonPropertyName("name")]
            public string Name { get; set; } = string.Empty;

            [JsonPropertyName("picture")]
            public string? Picture { get; set; }
        }
    }
}
