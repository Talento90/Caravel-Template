using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Caravel.Errors;
using CaravelTemplate.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace CaravelTemplate.Core.Books.Queries
{
    public class GetBooksQuery : IRequest<GetBooksQueryResponse>
    {
        public int Page { get; set; }
        public int Skip { get; set; }

        public GetBooksQuery()
        {
            Page = 10;
            Skip = 0;
        }

        public class Validator : AbstractValidator<GetBooksQuery>
        {
            public Validator()
            {
                RuleFor(p => p.Page)
                    .GreaterThan(0);
                RuleFor(p => p.Skip)
                    .GreaterThan(0)
                    .LessThanOrEqualTo(50);
            }
        }

        public class Handler : IRequestHandler<GetBooksQuery, GetBooksQueryResponse>
        {
            private readonly CaravelTemplateDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(CaravelTemplateDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<GetBooksQueryResponse> Handle(GetBooksQuery request, CancellationToken ct)
            {
                var books = await _mapper.ProjectTo<BookModel>(
                    _dbContext.Books
                        .Skip(request.Skip)
                        .Take(request.Page)
                ).ToListAsync(ct);
                
                return new GetBooksQueryResponse.Success(books);
            }
        }
    }
}