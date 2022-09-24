using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Caravel.ApplicationContext;
using Caravel.Errors;
using CaravelTemplate.Core.Identity;
using MediatR;

namespace CaravelTemplate.Core.Users.Queries
{
    public sealed record GetUserQuery : IRequest<GetUserResponse>
    {
        public class Handler : IRequestHandler<GetUserQuery, GetUserResponse>
        {
            private readonly IIdentityService _identityService;
            private readonly IMapper _mapper;
            private readonly ApplicationContext _appContext;

            public Handler(IIdentityService identityService, IAppContextAccessor appContextAccessor, IMapper mapper)
            {
                _identityService = identityService;
                _mapper = mapper;
                _appContext = appContextAccessor.Context;
            }

            public async Task<GetUserResponse> Handle(GetUserQuery request, CancellationToken ct)
            {
                var user = await _identityService.GetUserByIdAsync(_appContext.UserId!.Value);

                if (user == null)
                    return new GetUserResponse.NotFound(
                        new Error(Errors.UserNotFound, "User does not exist.")
                    );

                return new GetUserResponse.Success(_mapper.Map<UserModel>(user));
            }
        }
    }
}