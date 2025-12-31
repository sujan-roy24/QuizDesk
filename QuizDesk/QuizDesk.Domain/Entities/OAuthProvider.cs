namespace QuizDesk.Domain.Entities
{
    public class OAuthProvider : BaseEntity
    {
        public string Name { get; private set; }
        public string ClientId { get; private set; }
        public string ClientSecret { get; private set; }
        public string RedirectUri { get; private set; }
        public string AuthorizationEndpoint { get; private set; }
        public string TokenEndpoint { get; private set; }
        public string UserInfoEndpoint { get; private set; }
        public string[] Scopes { get; private set; }
        public bool IsActive { get; private set; }

        private OAuthProvider() { }

        public static OAuthProvider Create (string name, string clientId, string clientSecret, string redirectUri, string authorizationEndpoint, string tokenEndpoint, string userInfoEndpoint, string[] scopes)
        {
            return new OAuthProvider
            {
                Name = name,
                ClientId = clientId,
                ClientSecret = clientSecret,
                RedirectUri = redirectUri,
                AuthorizationEndpoint = authorizationEndpoint,
                TokenEndpoint = tokenEndpoint,
                UserInfoEndpoint = userInfoEndpoint,
                Scopes = scopes,
                IsActive = true,
            };
        }

        public void Update(string clientId, string clientSecret, string redirectUri, string[] scopes)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            RedirectUri = redirectUri;
            Scopes = scopes;
            MarkUpdated();
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
