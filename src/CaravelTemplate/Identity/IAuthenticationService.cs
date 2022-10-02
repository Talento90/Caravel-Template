using System.Threading;
using System.Threading.Tasks;
using Caravel.Functional;

namespace CaravelTemplate.Identity
{
    public interface IAuthenticationService
    {
        Task<Result<AccessToken>> LoginUserAsync(string username, string password, CancellationToken ct);
        Task<Result<AccessToken>> RefreshTokenAsync(string accessToken, string refreshToken, CancellationToken ct);
    }
}