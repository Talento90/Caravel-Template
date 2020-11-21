namespace CaravelTemplate.Core.Authentication
{
    public sealed record AccessTokenModel
    {
        public string AccessToken { get; init; }
        public string RefreshToken { get; init; }
        public int ExpiresIn { get; init; }
        
        public AccessTokenModel(string accessToken, int expiresIn, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpiresIn = expiresIn;
        }
    }
}