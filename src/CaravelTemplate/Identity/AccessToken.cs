namespace CaravelTemplate.Identity
{
    public record AccessToken(string Token, int ExpiresIn, string RefreshToken);
}