namespace CaravelTemplate.Core.Interfaces.Authentication
{
    public record AccessToken(string Token, int ExpiresIn, string RefreshToken);
}