namespace CaravelTemplate.Core.Authentication
{
    public class AccessTokenModel
    {
        public string AccessToken { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
        public int ExpiresIn { get; set; }

        public AccessTokenModel() { }

        public AccessTokenModel(string accessToken, int expiresIn, string refreshToken)
        {
            AccessToken = accessToken;
            RefreshToken = refreshToken;
            ExpiresIn = expiresIn;
        }
    }
}