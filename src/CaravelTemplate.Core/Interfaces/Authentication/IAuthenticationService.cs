using System.Threading;
using System.Threading.Tasks;
using Caravel.Errors;
using Caravel.Functional;

namespace CaravelTemplate.Core.Interfaces.Authentication
{
    public interface IAuthenticationService
    {
        Task<Either<Error, AccessToken>> LoginUserAsync(string username, string password, CancellationToken ct);
        Task<Either<Error, AccessToken>> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken ct);
    }
}