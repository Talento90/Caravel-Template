using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Caravel.AspNetCore.Authentication;
using Caravel.Errors;
using CaravelTemplate.Entities;
using CaravelTemplate.Infrastructure.Data;
using CaravelTemplate.Infrastructure.Identity;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace CaravelTemplate.Core.Authentication.Commands
{
    public class RefreshTokenCommand : IRequest<RefreshTokenCommandResponse>
    {
        public string RefreshToken { get; set; } = null!;
        public string AccessToken { get; set; } = null!;

        public class Validator : AbstractValidator<RefreshTokenCommand>
        {
            public Validator()
            {
                RuleFor(p => p.RefreshToken).NotEmpty();
                RuleFor(p => p.AccessToken).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<RefreshTokenCommand, RefreshTokenCommandResponse>
        {
            private readonly JwtIssuerSettings _jwtIssuerSettings;
            private readonly UserManager<User> _userManager;
            private readonly IJwtManager _jwtManager;
            private readonly ITokenFactory _tokenFactory;
            private readonly CaravelTemplateDbContext _dbContext;

            public Handler(
                UserManager<User> userManager,
                IJwtManager jwtManager,
                ITokenFactory tokenFactory,
                CaravelTemplateDbContext dbContext,
                IOptions<JwtIssuerSettings> jwtIssuerSettings)
            {
                _userManager = userManager;
                _jwtManager = jwtManager;
                _tokenFactory = tokenFactory;
                _dbContext = dbContext;
                _jwtIssuerSettings = jwtIssuerSettings.Value;
            }

            public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommand request, CancellationToken ct)
            {
                var claims = _jwtManager.GetClaims(request.AccessToken, _jwtIssuerSettings.IssuerSigningKey);

                if (claims == null)
                {
                    return new RefreshTokenCommandResponse.InvalidAccessToken(
                        new Error("invalid_access_token", "Invalid access token.")
                    );
                }

                var id = claims.Claims.First(c => c.Type == "id");

                var user = await _userManager.FindByIdAsync(id.Value);
                
                var existingRefreshToken = _dbContext.RefreshTokens.FirstOrDefault(t => t.Token == request.RefreshToken);

                if (existingRefreshToken == null)
                {
                    return new RefreshTokenCommandResponse.RefreshTokenNotFound(
                        new Error("refresh_token_not_found", "Refresh token does not exist.")
                    );
                }

                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtManager.GenerateAccessToken(user.Id.ToString(), user.UserName, new List<Claim>()
                {
                    new ("roles", string.Join(',', roles))
                });
                
                var refreshToken = await _tokenFactory.GenerateToken();
                
                _dbContext.Remove(existingRefreshToken);
                await _dbContext.RefreshTokens.AddAsync(new RefreshToken(
                    refreshToken,
                    DateTime.Now.Date.AddDays(30),
                    user.Id
                ), ct);

                await _dbContext.SaveChangesAsync(ct);

                return new RefreshTokenCommandResponse.Success(new AccessTokenModel(
                    token.Token,
                    token.ExpiresIn,
                    refreshToken
                ));
            }
        }
    }
}