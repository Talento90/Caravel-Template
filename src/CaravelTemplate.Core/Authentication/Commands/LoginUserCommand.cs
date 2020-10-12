using System;
using System.Linq;
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

namespace CaravelTemplate.Core.Authentication.Commands
{
    public class LoginUserCommand : IRequest<LoginUserCommandResponse>
    {
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;

        public class Validator : AbstractValidator<LoginUserCommand>
        {
            public Validator()
            {
                RuleFor(p => p.Username).NotEmpty();
                RuleFor(p => p.Password).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<LoginUserCommand, LoginUserCommandResponse>
        {
            private readonly UserManager<User> _userManager;
            private readonly IJwtManager _jwtManager;
            private readonly ITokenFactory _tokenFactory;
            private readonly CaravelTemplateDbContext _dbContext;

            public Handler(
                UserManager<User> userManager,
                IJwtManager jwtManager,
                ITokenFactory tokenFactory,
                CaravelTemplateDbContext dbContext
            )
            {
                _userManager = userManager;
                _jwtManager = jwtManager;
                _tokenFactory = tokenFactory;
                _dbContext = dbContext;
            }

            public async Task<LoginUserCommandResponse> Handle(LoginUserCommand request, CancellationToken ct)
            {
                var user = await _userManager.FindByNameAsync(request.Username);

                if (user == null)
                {
                    return new LoginUserCommandResponse.NotFound(new Error(Errors.UserNotFound,
                        $"User {request.Username} does not exist."));
                }
                
                if (!await _userManager.CheckPasswordAsync(user, request.Password))
                {
                    return new LoginUserCommandResponse.InvalidPassword(new Error(
                        Errors.MatchPassword,
                        $"Password does not match.")
                    );
                }

                var refreshToken = await _tokenFactory.GenerateToken();


                await _dbContext.RefreshTokens.AddAsync(new RefreshToken(
                    refreshToken,
                    DateTime.UtcNow.AddDays(30),
                    user.Id
                ), ct);


                await _dbContext.SaveChangesAsync(ct);

                var roles = await _userManager.GetRolesAsync(user);
                var token = _jwtManager.GenerateAccessToken(user.Id.ToString(), user.UserName, roles.ToArray());

                return new LoginUserCommandResponse.Success(new AccessTokenModel(
                    token.Token,
                    token.ExpiresIn,
                    refreshToken
                ));
            }
        }
    }
}