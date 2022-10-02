using System.Threading;
using System.Threading.Tasks;
using Caravel.AspNetCore.Http;
using CaravelTemplate.Core.Users;
using CaravelTemplate.Core.Users.Commands;
using CaravelTemplate.Core.Users.Queries;
using CaravelTemplate.Entities;
using CaravelTemplate.Identity;
using CaravelTemplate.WebApi.Extensions;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CaravelTemplate.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UsersController : BaseController
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Authorize(Roles = Roles.User)]
        [HttpGet("profile", Name = "GetUserAsync")]
        public async Task<ActionResult<UserModel>> GetUserAsync(CancellationToken ct)
        {
            var response = await _mediator.Send(new GetUserQuery(), ct);

            return response.Match(
                success => Ok(success.Response),
                notFound => NotFound(notFound.Error));
        }

        [HttpPost]
        public async Task<ActionResult<UserModel>> CreateUserAsync(CreateUserCommand create, CancellationToken ct)
        {
            var response = await _mediator.Send(create, ct);
            
            return response.Match(
                success => CreatedAtRoute(nameof(GetUserAsync), null, success.Response),
                invalid => BadRequest(invalid.Error)
            );
        }
    }
}