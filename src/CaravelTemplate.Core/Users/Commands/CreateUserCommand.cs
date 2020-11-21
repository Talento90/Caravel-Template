using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Caravel.Errors;
using CaravelTemplate.Entities;
using CaravelTemplate.Infrastructure.Identity;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace CaravelTemplate.Core.Users.Commands
{
    public sealed record CreateUserCommand : IRequest<CreateUserCommandResponse>
    {
        public string FirstName { get; init; }
        public string LastName { get; init; }
        public string Email { get; init; }
        public string Username { get; init; }
        public string Password { get; init; }
        public string ConfirmPassword { get; init; }

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
            private readonly UserManager<User> _userManager;
            private readonly IMapper _mapper;

            public Handler(UserManager<User> userManager, IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<CreateUserCommandResponse> Handle(CreateUserCommand request, CancellationToken ct)
            {
                var user = new User
                {
                    Email = request.Email,
                    UserName = request.Username,
                    FirstName = request.FirstName,
                    LastName = request.LastName
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    return new CreateUserCommandResponse.InvalidUser(
                        new Error(Errors.UserCreation, "Error creating user.")
                            .SetDetails(string.Join(',', result.Errors.Select(e => e.Description)))
                    );
                }

                var roleResult = await _userManager.AddToRoleAsync(user, Roles.User);

                if (!roleResult.Succeeded)
                {
                    return new CreateUserCommandResponse.InvalidUser(
                        new Error(Errors.UserCreation, "Error associating role to user.")
                            .SetDetails(string.Join(',', result.Errors.Select(e => e.Description)))
                    );
                }

                return new CreateUserCommandResponse.Success(_mapper.Map<UserModel>(user));
            }
        }
    }
}