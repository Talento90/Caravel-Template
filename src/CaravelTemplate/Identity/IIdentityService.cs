using System;
using System.Threading.Tasks;
using Caravel.Errors;
using Caravel.Functional;
using CaravelTemplate.Entities;

namespace CaravelTemplate.Identity
{
    public interface IIdentityService
    {
        Task<User?> GetUserByIdAsync(Guid userId);
        
        Task<Result<User>> CreateUserAsync(CreateUser createUser);

        Task<Result> DeleteUserAsync(Guid userId);
    }
}