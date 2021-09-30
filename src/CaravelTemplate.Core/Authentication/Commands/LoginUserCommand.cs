using System.Threading;
using System.Threading.Tasks;
using Caravel.Errors;
using Caravel.Functional;
using FluentValidation;
using MediatR;

namespace CaravelTemplate.Core.Authentication.Commands
{
    public sealed record LoginUserCommand : IRequest<LoginUserCommandResponse>
    {
        public string Username { get; init; } = null!;
        public string Password { get; init; } = null!;

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
            private readonly IAuthenticationService _authService;

            public Handler(IAuthenticationService authService)
            {
                _authService = authService;
            }

            public async Task<LoginUserCommandResponse> Handle(LoginUserCommand request, CancellationToken ct)
            {
                var result = await _authService.LoginUserAsync(request.Username, request.Password, ct);

                return result.Fold<Error, AccessToken, LoginUserCommandResponse>(
                    err => new LoginUserCommandResponse.InvalidPassword(err),
                    token => new LoginUserCommandResponse.Success(new AccessTokenModel(
                        token.Token,
                        token.ExpiresIn,
                        token.RefreshToken
                    )));
            }
        }
    }
}