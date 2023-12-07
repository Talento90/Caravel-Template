using AutoMapper;
using Caravel.ApplicationContext;
using CaravelTemplate.Errors;
using CaravelTemplate.Identity;
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

            public Handler(IIdentityService identityService, IApplicationContextAccessor appContextAccessor, IMapper mapper)
            {
                _identityService = identityService;
                _mapper = mapper;
                _appContext = appContextAccessor.Context;
            }

            public async Task<GetUserResponse> Handle(GetUserQuery request, CancellationToken ct)
            {
                var userId = _appContext.UserId!.Value;
                var user = await _identityService.GetUserByIdAsync(userId);

                if (user == null)
                    return new GetUserResponse.NotFound(UserErrors.NotFound(userId));

                return new GetUserResponse.Success(_mapper.Map<UserModel>(user));
            }
        }
    }
}