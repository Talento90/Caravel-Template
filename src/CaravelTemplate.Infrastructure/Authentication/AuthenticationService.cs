using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Caravel.AspNetCore.Authentication;
using Caravel.Errors;
using Caravel.Functional;
using CaravelTemplate.Core;
using CaravelTemplate.Core.Interfaces.Authentication;
using CaravelTemplate.Entities;
using CaravelTemplate.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using AccessToken = CaravelTemplate.Core.Interfaces.Authentication.AccessToken;

namespace CaravelTemplate.Infrastructure.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<Identity.User> _userManager;
        private readonly IJwtManager _jwtManager;
        private readonly ITokenFactory _tokenFactory;
        private readonly JwtIssuerSettings _jwtIssuerSettings;
        private readonly CaravelTemplateTemplateDbContext _templateDbContext;

        public AuthenticationService(
            UserManager<Identity.User> userManager,
            IJwtManager jwtManager,
            ITokenFactory tokenFactory,
            CaravelTemplateTemplateDbContext templateDbContext,
            IOptions<JwtIssuerSettings> jwtIssuerSettings)
        {
            _userManager = userManager;
            _jwtManager = jwtManager;
            _tokenFactory = tokenFactory;
            _templateDbContext = templateDbContext;
            _jwtIssuerSettings = jwtIssuerSettings.Value;
        }

        public async Task<Either<Error, AccessToken>> LoginUserAsync(string username, string password,
            CancellationToken ct)
        {
            var user = await _userManager.FindByNameAsync(username);

            if (user == null)
            {
                return Either.Left<Error, AccessToken>(new Error(
                    Errors.UserNotFound,
                    $"User {username} does not exist.")
                );
            }

            if (!await _userManager.CheckPasswordAsync(user, password))
            {
                return Either.Left<Error, AccessToken>(new Error(
                    Errors.MatchPassword,
                    $"Password does not match.")
                );
            }

            var refreshToken = await _tokenFactory.GenerateToken();

            await _templateDbContext.RefreshTokens.AddAsync(new RefreshToken(
                refreshToken,
                DateTime.UtcNow.AddDays(30),
                user.Id
            ), ct);


            await _templateDbContext.SaveChangesAsync(ct);

            var roles = await _userManager.GetRolesAsync(user);
            var token = _jwtManager.GenerateAccessToken(user.Id.ToString(), username, new List<Claim>()
            {
                new("roles", string.Join(',', roles))
            });

            return Either.Right<Error, AccessToken>(new AccessToken(
                token.Token,
                token.ExpiresIn,
                refreshToken
            ));
        }

        public async Task<Either<Error, AccessToken>> RefreshTokenAsync(string accessToken, string refreshToken,
            CancellationToken ct)
        {
            var claims = _jwtManager.GetClaims(accessToken, _jwtIssuerSettings.IssuerSigningKey);

            if (claims == null)
            {
                return Either.Left<Error, AccessToken>(
                    new Error("invalid_access_token", "Invalid access token.")
                );
            }

            var id = claims.Claims.First(c => c.Type == "id");

            var user = await _userManager.FindByIdAsync(id.Value);

            var existingRefreshToken = _templateDbContext.RefreshTokens.FirstOrDefault(t => t.Token == refreshToken);

            if (existingRefreshToken == null)
            {
                return Either.Left<Error, AccessToken>(
                    new Error("refresh_token_not_found", "Refresh token does not exist.")
                );
            }

            var roles = await _userManager.GetRolesAsync(user);

            var token = _jwtManager.GenerateAccessToken(user.Id.ToString(), user.UserName, new List<Claim>()
            {
                new("roles", string.Join(',', roles))
            });

            var newRefreshToken = await _tokenFactory.GenerateToken();

            _templateDbContext.Remove(existingRefreshToken);

            await _templateDbContext.RefreshTokens.AddAsync(new RefreshToken(
                newRefreshToken,
                DateTime.Now.Date.AddDays(30),
                user.Id
            ), ct);

            await _templateDbContext.SaveChangesAsync(ct);

            return Either.Right<Error, AccessToken>(new AccessToken(
                token.Token,
                token.ExpiresIn,
                newRefreshToken
            ));
        }
    }
}