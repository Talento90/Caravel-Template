using System;
using System.Linq;
using System.Threading.Tasks;
using Caravel.Errors;
using Caravel.Functional;
using CaravelTemplate.Core;
using CaravelTemplate.Core.Interfaces.Identity;
using Microsoft.AspNetCore.Identity;

namespace CaravelTemplate.Infrastructure.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;
            
        public IdentityService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        
        public async Task<Entities.User?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            return user != null ? MapUser(user) : null;
        }

        public Task<bool> IsInRoleAsync(string userId, string role)
        {
            throw new NotImplementedException();
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