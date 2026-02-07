using Microsoft.Extensions.Logging;
using QuizDesk.Application.Abstractions;
using QuizDesk.Application.DTOs.Auth;
using QuizDesk.Application.Interfaces.Auth;
using QuizDesk.Application.Mappings;
using QuizDesk.Domain.Entities;
using QuizDesk.Domain.Enums;
using QuizDesk.Domain.Interfaces;
using QuizDesk.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations;

namespace QuizDesk.Application.Services.Auth
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _logger = logger;

        }
        public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
        {
            _logger.LogInformation("Registering user with email: {Email}", request.Email);
            
            if (request.Password != request.ConfirmPassword)
                throw new ValidationException("Passwords do not match");

            var email = Email.Create(request.Email);
            var password = Password.Create(request.Password);

            if (await _userRepository.ExistsByEmailAsync(email))
                throw new ValidationException("Email already exists");

            var parseUserRole = (string role) =>
            {
                if (string.IsNullOrWhiteSpace(role))
                    throw new ArgumentException("Role is required");

                return Enum.TryParse<UserRole>(role, ignoreCase: true, out var parsed)
                    ? parsed
                    : throw new ArgumentException($"Invalid role: {role}. Allowed: Admin, Setter, Participant");
            };
            var user = User.Create(
                email, 
                request.FullName,
                parseUserRole(request.Role), 
                AuthMethod.Password);

            user.SetPassword(_passwordHasher.HashPassword(password));
            await _userRepository.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("User registered successfully with ID: {UserId}", user.Id);

            var tokens = await _tokenService.GenerateTokens(user);
            return new AuthResponse
            {
                User = user.ToDto(),
                Tokens = tokens,
                IsNewUser = true
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {

            _logger.LogInformation("Login attempt for email: {Email}", request.Email);
            
            var email = Email.Create(request.Email);
            var password = Password.Create(request.Password);

            var user = await _userRepository.GetByEmailAsync(email);
            
            if (user == null || user.AuthMethod != AuthMethod.Password)
            {
                _logger.LogWarning("Login failed - user not found or inactive: {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            if (user.AuthMethod == AuthMethod.Google)
            {
                _logger.LogWarning("Login failed - user uses Google auth: {Email}", request.Email);
                throw new UnauthorizedAccessException("Please use Google sign-in");
            }

            if (!_passwordHasher.VerifyPassword(password, user.PasswordHash!))
            {
                _logger.LogWarning("Login failed - invalid password for: {Email}", request.Email);
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            user.UpdateLastLogin();
            await _userRepository.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();
            _logger.LogInformation("Login successful for user ID: {UserId}", user.Id);

            var tokens = await _tokenService.GenerateTokens(user);

            return new AuthResponse
            {
                User = user.ToDto(),
                Tokens = tokens,
                IsNewUser = false
            };
        }

        public async Task<AuthResponse> RefreshTokenAsync(string refreshToken)
        {
            _logger.LogInformation("Refreshing token");
            var tokens = await _tokenService.RefreshToken(refreshToken);
            return new AuthResponse
            {
                User = null!,
                Tokens = tokens,
                IsNewUser = false
            };
        }
        public async Task<bool> LogoutAsync(string refreshToken)
        {
            _logger.LogInformation("Logout requested");
            return await _tokenService.RevokeToken(refreshToken);
        }

        public async Task<UserDto> GetProfileAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"User with id {id} not found.");
            
            return user.ToDto();
        }
    }
}
