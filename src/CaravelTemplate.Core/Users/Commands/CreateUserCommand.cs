using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Caravel.Errors;
using Caravel.Functional;
using CaravelTemplate.Core.Interfaces.Identity;
using CaravelTemplate.Entities;
using FluentValidation;
using MediatR;

namespace CaravelTemplate.Core.Users.Commands
{
    public sealed record CreateUserCommand : IRequest<CreateUserCommandResponse>
    {
        public string FirstName { get; init; } = null!;
        public string LastName { get; init; } = null!;
        public string Email { get; init; } = null!;
        public string Username { get; init; } = null!;
        public string Password { get; init; } = null!;
        public string ConfirmPassword { get; init; } = null!;

        public class Validator : AbstractValidator<CreateUserCommand>
        {
            public Validator()
            {
                RuleFor(p => p.FirstName).NotEmpty();
                RuleFor(p => p.LastName).NotEmpty();
                RuleFor(p => p.Email).EmailAddress();
                RuleFor(p => p.Username).NotEmpty();
                RuleFor(p => p.Password).NotEmpty();
                RuleFor(p => p.ConfirmPassword).NotEmpty();

                RuleFor(p => p)
                    .Must(p => p.Password.Equals(p.ConfirmPassword))
                    .WithMessage("Password does not match.");
            }
        }

        public class Handler : IRequestHandler<CreateUserCommand, CreateUserCommandResponse>
        {
            private readonly IIdentityService _identityService;
            private readonly IMapper _mapper;

            public Handler(IIdentityService identityService, IMapper mapper)
            {
                _identityService = identityService;
                _mapper = mapper;
            }

            public async Task<CreateUserCommandResponse> Handle(CreateUserCommand request, CancellationToken ct)
            {
                Either<Error, User> result = await _identityService.CreateUserAsync(new CreateUser(
                    request.Username,
                    request.FirstName,
                    request.LastName,
                    request.Email,
                    request.Password,
                    new [] {Roles.User}
                    ));

                return result.Fold<Error, User, CreateUserCommandResponse>(
                    (Error err) => new CreateUserCommandResponse.InvalidUser(err),
                        (User user) => new CreateUserCommandResponse.Success(_mapper.Map<UserModel>(user))
                    );
            }
        }
    }
}