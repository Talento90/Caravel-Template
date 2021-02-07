using System;
using System.Threading.Tasks;
using Caravel.Errors;
using Caravel.Functional;
using CaravelTemplate.Entities;

namespace CaravelTemplate.Core.Interfaces.Identity
{
    public interface IIdentityService
    {
        Task<User?> GetUserByIdAsync(Guid userId);

        Task<bool> IsInRoleAsync(string userId, string role);
        
        Task<Either<Error, User>> CreateUserAsync(CreateUser createUser);
    }
}