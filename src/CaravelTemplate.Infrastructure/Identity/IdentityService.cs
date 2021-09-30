using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Caravel.Errors;
using Caravel.Functional;
using Caravel.MediatR.Security;
using CaravelTemplate.Core;
using CaravelTemplate.Core.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace CaravelTemplate.Infrastructure.Identity
{
    public class IdentityService : IIdentityService, IAuthorizer
    {
        private readonly UserManager<User> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserClaimsPrincipalFactory<User> _userClaimsPrincipalFactory;

        public IdentityService(UserManager<User> userManager, IAuthorizationService authorizationService, IUserClaimsPrincipalFactory<User> userClaimsPrincipalFactory)
        {
            _userManager = userManager;
            _authorizationService = authorizationService;
            _userClaimsPrincipalFactory = userClaimsPrincipalFactory;
        }

        public async Task<Entities.User?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return user != null ? MapUser(user) : null;
        }

        public async Task<bool> IsInRoleAsync(Guid userId, string role, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<bool> AuthorizeAsync(Guid userId, string policy, CancellationToken ct)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            var principal = await _userClaimsPrincipalFactory.CreateAsync(user);

            var result = await _authorizationService.AuthorizeAsync(principal, policy);

            return result.Succeeded;
        }

        public async Task<Either<Error, Entities.User>> CreateUserAsync(CreateUser createUser)
        {
            var user = new User
            {
                Email = createUser.Email,
                UserName = createUser.Username,
                FirstName = createUser.FirstName,
                LastName = createUser.LastName
            };

            var result = await _userManager.CreateAsync(user, createUser.Password);

            if (!result.Succeeded)
            {
                return Result.Error<Entities.User>(new Error(Errors.UserCreation, "Error creating user.")
                    .SetDetails(string.Join(',', result.Errors.Select(e => e.Description))));
            }

            var rolesResult = await _userManager.AddToRolesAsync(user, createUser.Roles);

            if (!rolesResult.Succeeded)
            {
                return Result.Error<Entities.User>(new Error(Errors.UserCreation, "Error creating user.")
                    .SetDetails(string.Join(',', result.Errors.Select(e => e.Description))));
            }

            return Result.Success(MapUser(user));
        }

        public async Task<Optional<Error>> DeleteUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return Optional.Some(new Error("user_not_found", "User does not exist"));
            }

            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded
                ? Optional.None<Error>()
                : Optional.Some(new Error("user_delete", string.Join(",", result.Errors.Select(e => e.Description))));
        }

        public async Task<Optional<Error>> UpdateUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                return Optional.Some(new Error("user_not_found", "User does not exist"));
            }

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded
                ? Optional.None<Error>()
                : Optional.Some(new Error("user_update", string.Join(",", result.Errors.Select(e => e.Description))));
        }

        private static Entities.User MapUser(User user) => new Entities.User()
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Created = user.Created,
            Updated = user.Updated,
            CreatedBy = user.CreatedBy,
            UpdatedBy = user.UpdatedBy
        };
    }
}