using QuizDesk.Application.Abstractions;
using QuizDesk.Application.Contracts.OAuth;
using QuizDesk.Application.DTOs.Auth;
using QuizDesk.Application.DTOs.Models;
using QuizDesk.Application.DTOs.OAuth;
using QuizDesk.Application.Interfaces.OAuth;
using QuizDesk.Application.Mappings;
using QuizDesk.Domain.Entities;
using QuizDesk.Domain.Enums;
using QuizDesk.Domain.Interfaces;
using QuizDesk.Domain.ValueObjects;
using System.Threading;

namespace QuizDesk.Application.Services.OAuth
{
    public class OAuthService : IOAuthService
    {
        private readonly IEnumerable<IOAuthProviderService> _providerServices;
        private readonly IUserRepository _userRepository;
        private readonly IOAuthRepository _oauthRepository;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISessionStore _sessionStore;

        public OAuthService(
            IEnumerable<IOAuthProviderService> providerServices,
            IUserRepository userRepository,
            IOAuthRepository oauthsRepository,
            ITokenService tokenService,
            IUnitOfWork unitOfWork,
            ISessionStore sessionStore
            ) 
        {
            _providerServices = providerServices;
            _userRepository = userRepository;
            _oauthRepository = oauthsRepository;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _sessionStore = sessionStore;
        }

        public async Task<AuthResponse> LoginAsync(OAuthRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code))
                throw new ArgumentException("OAuth code is required.");

            if (string.IsNullOrWhiteSpace(request.State))
                throw new ArgumentException("Provider state is required");

            var provider = _providerServices.FirstOrDefault(p => p.ProviderKey == request.State);



            var tokens = await provider.ExchangeCodeAsync(request.Code);


            var userInfo = await provider.GetUserInfoAsync(tokens.AccessToken);

            
            var oauthAccount = await _oauthRepository
                .GetByProviderUserIdAsync(provider.ProviderKey, userInfo.ProviderUserId);

            User user;
            var isNewUser = false;

            if (oauthAccount != null)
            {
                user = await _userRepository.GetByIdAsync(oauthAccount.UserId) 
                    ?? throw new InvalidOperationException("User not found for OAuth account");
            }
            else
            {
                var email = Email.Create(userInfo.Email);
                user = await _userRepository.GetByEmailAsync(email);

                if (user == null)
                {
                    user = User.Create(email, userInfo.FullName, UserRole.Participant, AuthMethod.Google);
                    
                    await _userRepository.AddAsync(user);
                    await _unitOfWork.SaveChangesAsync();
                }

                var newOauth = UserOAuthAccount.Create(
                    user.Id,
                    provider.ProviderKey,
                    userInfo.ProviderUserId,
                    email);

                await _oauthRepository.AddAsync(newOauth);
            }
            await _unitOfWork.SaveChangesAsync();

            await _sessionStore.SaveAsync(new SessionData
            {
                UserId = user.Id,
                Provider = request.State,
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken,
                ExpiresAt = tokens.ExpiresAt
            });

            var token = await _tokenService.GenerateTokens(user);

            return new AuthResponse
            {
                User = user.ToDto(),
                Tokens = token,
                IsNewUser = isNewUser
            };
        }
    }
}
