using System.Security.Claims;
using Caravel.AspNetCore.Authentication;
using Caravel.Errors;
using Caravel.Functional;
using CaravelTemplate.Errors;
using CaravelTemplate.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CaravelTemplate.Identity.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtManager _jwtManager;
        private readonly ITokenFactory _tokenFactory;
        private readonly JwtIssuerSettings _jwtIssuerSettings;
        private readonly CaravelTemplateIdentityDbContext _identityDbContext;

        public AuthenticationService(
            UserManager<User> userManager,
            IJwtManager jwtManager,
            ITokenFactory tokenFactory,
            CaravelTemplateIdentityDbContext identityDbContext,
            IOptions<JwtIssuerSettings> jwtIssuerSettings)
        {
            _userManager = userManager;
            _jwtManager = jwtManager;
            _tokenFactory = tokenFactory;
            _identityDbContext = identityDbContext;
            _jwtIssuerSettings = jwtIssuerSettings.Value;
        }

        public async Task<Result<AccessToken>> LoginUserAsync(string username, string password, CancellationToken ct)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return Result<AccessToken>.Failure(UserErrors.NotFound(username));
            }

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                return Result<AccessToken>.Failure(UserErrors.PasswordMismatch());
            }

            var refreshToken = await _tokenFactory.GenerateToken();

            await _identityDbContext.RefreshTokens.AddAsync(new RefreshToken(
                refreshToken,
                DateTime.UtcNow.AddDays(30),
                user.Id
            ), ct);


            await _identityDbContext.SaveChangesAsync(ct);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtManager.GenerateAccessToken(user.Id.ToString(), username, new List<Claim>()
            {
                new("roles", string.Join(',', roles))
            });

            return Result<AccessToken>.Success(new AccessToken(
                token.Token,
                token.ExpiresIn,
                refreshToken
            ));
        }

        public async Task<Result<AccessToken>> RefreshTokenAsync(string accessToken, string refreshToken,
            CancellationToken ct)
        {
            var claims = _jwtManager.GetClaims(accessToken, _jwtIssuerSettings.IssuerSigningKey);

            if (claims == null)
            {
                return Result<AccessToken>.Failure(
                    new Error("invalid_access_token", "Invalid access token.")
                );
            }

            var id = claims.Claims.First(c => c.Type == "id");

            var user = await _userManager.FindByIdAsync(id.Value);

            var existingRefreshToken = _identityDbContext.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);

            if (existingRefreshToken == null)
            {
                return Result<AccessToken>.Failure(
                    new Error("refresh_token_not_found", "Refresh token does not exist.")
                );
            }

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtManager.GenerateAccessToken(user.Id.ToString(), user.UserName, new List<Claim>()
            {
                new("roles", string.Join(',', roles))
            });

            var newRefreshToken = await _tokenFactory.GenerateToken();

            _identityDbContext.Remove(existingRefreshToken);

            await _identityDbContext.RefreshTokens.AddAsync(new RefreshToken(
                newRefreshToken,
                DateTime.Now.Date.AddDays(30),
                user.Id
            ), ct);

            await _identityDbContext.SaveChangesAsync(ct);

            return Result<AccessToken>.Success(new AccessToken(
                token.Token,
                token.ExpiresIn,
                newRefreshToken
            ));
        }
    }
}