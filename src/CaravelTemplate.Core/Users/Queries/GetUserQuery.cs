using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Caravel.AppContext;
using Caravel.Errors;
using CaravelTemplate.Infrastructure.Data;
using FluentValidation;
using MediatR;
using AppContext = Caravel.AppContext.AppContext;

namespace CaravelTemplate.Core.Users.Queries
{
    public sealed record GetUserQuery : IRequest<GetUserResponse>
    {
        public class Handler : IRequestHandler<GetUserQuery, GetUserResponse>
        {
            private readonly CaravelTemplateDbContext _dbContext;
            private readonly IMapper _mapper;
            private readonly AppContext _appContext;

            public Handler(CaravelTemplateDbContext dbContext, IAppContextAccessor appContextAccessor, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
                _appContext = appContextAccessor.Context;
            }

            public async Task<GetUserResponse> Handle(GetUserQuery request, CancellationToken ct)
            {
                var user = await _dbContext.Users.FindAsync(_appContext.UserId);

                if (user == null)
                    return new GetUserResponse.NotFound(
                        new Error(Errors.UserNotFound, "User does not exist.")
                    );

                return new GetUserResponse.Success(_mapper.Map<UserModel>(user));
            }
        }
    }
}