using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using QuizDesk.Application.Abstractions;
using QuizDesk.Application.Common.Settings;
using QuizDesk.Application.DTOs.Auth;
using QuizDesk.Domain.Entities;
using QuizDesk.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace QuizDesk.Infrastructure.Services.Identity
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly IDistributedCache _cache;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<TokenService> _logger;

        public TokenService(
            IOptions<JwtSettings> jwtSettings,
            IDistributedCache cache,
            IUserRepository userRepository,
            ILogger<TokenService> logger)
        {
            _jwtSettings = jwtSettings.Value;
            _cache = cache;
            _userRepository = userRepository;
            _logger = logger;


        }
        public async Task<TokenResponse> GenerateTokens(User user)
        {
            var accessToken = GenerateAccessToken(user);
            var refreshtoken = GenerateRefreshToken();

            await StoreRefreshTokenAsync(user.Id, refreshtoken);
            return new TokenResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshtoken,
                AccessTokenExpiry = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiryDays)
            };
        }

        public async Task<TokenResponse> RefreshToken(string refreshToken)
        {
            var userIdStr = await GetUserIdFromRefreshTokenAsync(refreshToken);

            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                _logger.LogWarning("Invalid or expired refresh token");
                throw new UnauthorizedAccessException("Session expired. Please login again.");
            }
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                await RemoveRefreshTokenAsync(refreshToken);
                throw new UnauthorizedAccessException("User account is no longer available.");
            }
            await RemoveRefreshTokenAsync(refreshToken);
            return await GenerateTokens(user);
        }

        public async Task<bool> RevokeToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken)) return false;

            await RemoveRefreshTokenAsync(refreshToken);
            return true;
        }

        public async Task<bool> ValidateToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token)) 
                return false;

            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_jwtSettings.Secret);

                var validationParamenters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _jwtSettings.Issuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtSettings.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                tokenHandler.ValidateToken(token, validationParamenters, out _);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Token validation failed");
                return false;
            }
        }

        #region Private Methods
        private string GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email.Value), // Using Value Object
                new(ClaimTypes.Role, user.Role.ToString()),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // Unique ID for this specific token
                new("fullname", user.FullName)
            };

            var token = new JwtSecurityToken(
                _jwtSettings.Issuer,
                _jwtSettings.Audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64]; // Increased entropy
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private async Task StoreRefreshTokenAsync(int userId, string refreshToken)
        {
            var cacheKey = GetCacheKey(refreshToken);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(_jwtSettings.RefreshTokenExpiryDays)
            };

            // We store as string for simplicity, but could be JSON object for more metadata
            await _cache.SetStringAsync(cacheKey, userId.ToString(), options);
        }

        private async Task<string?> GetUserIdFromRefreshTokenAsync(string refreshToken)
        {
            return await _cache.GetStringAsync(GetCacheKey(refreshToken));
        }

        private async Task RemoveRefreshTokenAsync(string refreshToken)
        {
            await _cache.RemoveAsync(GetCacheKey(refreshToken));
        }

        private static string GetCacheKey(string token) => $"auth:refresh_token:{token}";
        
        #endregion
    }
}
