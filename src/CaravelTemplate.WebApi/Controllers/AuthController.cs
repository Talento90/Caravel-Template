using System.Threading;
using System.Threading.Tasks;
using CaravelTemplate.Core.Authentication;
using CaravelTemplate.Core.Authentication.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CaravelTemplate.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AuthController : BaseController
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [HttpPost("login")]
        public async Task<ActionResult<AccessTokenModel>> LoginAsync(LoginUserCommand login, CancellationToken ct)
        {
            var response = await _mediator.Send(login, ct);

            return response.Match(
                success => Ok(success.Response),
                notFound => BadRequest(notFound.Error),
                invalidPassword => BadRequest(invalidPassword.Error)
            );
        }
        
        [HttpPost("refresh-token")]
        public async Task<ActionResult<AccessTokenModel>> RefreshTokenAsync(RefreshTokenCommand refreshToken, CancellationToken ct)
        {
            var response = await _mediator.Send(refreshToken, ct);
                
            return response.Match(
                success => Ok(success.Response),
                refreshTokenNotFound => BadRequest(refreshTokenNotFound.Error),
                invalidAccessToken => BadRequest(invalidAccessToken.Error)
            );
        }
    }
}