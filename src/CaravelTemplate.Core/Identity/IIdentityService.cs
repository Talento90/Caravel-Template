using System;
using System.Threading.Tasks;
using Caravel.Errors;
using Caravel.Functional;
using CaravelTemplate.Entities;

namespace CaravelTemplate.Core.Identity
{
    public interface IIdentityService
    {
        Task<User?> GetUserByIdAsync(Guid userId);
        
        Task<Either<Error, User>> CreateUserAsync(CreateUser createUser);

        Task<Optional<Error>> DeleteUserAsync(Guid userId);
    }
}