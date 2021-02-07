using System.Threading;
using System.Threading.Tasks;
using Caravel.Errors;
using Caravel.Functional;
using CaravelTemplate.Core.Interfaces.Authentication;
using FluentValidation;
using MediatR;

namespace CaravelTemplate.Core.Authentication.Commands
{
    public sealed record RefreshTokenCommand : IRequest<RefreshTokenCommandResponse>
    {
        public string RefreshToken { get; init; } = null!;
        public string AccessToken { get; init; } = null!;

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
            private readonly IAuthenticationService _authService;

            public Handler(IAuthenticationService authService)
            {
                _authService = authService;
            }

            public async Task<RefreshTokenCommandResponse> Handle(RefreshTokenCommand request, CancellationToken ct)
            {
                var result = await _authService.RefreshTokenAsync(request.AccessToken, request.RefreshToken, ct);

                return result.Fold<Error, AccessToken, RefreshTokenCommandResponse>(
                    err => new RefreshTokenCommandResponse.InvalidAccessToken(err),
                    token => new RefreshTokenCommandResponse.Success(new AccessTokenModel(
                        token.Token,
                        token.ExpiresIn,
                        token.RefreshToken
                    )));
            }
        }
    }
}