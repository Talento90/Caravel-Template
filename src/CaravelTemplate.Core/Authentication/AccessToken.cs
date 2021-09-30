namespace CaravelTemplate.Core.Authentication
{
    public record AccessToken(string Token, int ExpiresIn, string RefreshToken);
}