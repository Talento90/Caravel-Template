using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Caravel.Errors;
using Caravel.Exceptions;
using Caravel.Functional;
using CaravelTemplate.Infrastructure.Data;
using FluentValidation;
using MediatR;
using OneOf;

namespace CaravelTemplate.Core.Books.Queries
{
    public class GetBookByIdQuery : IRequest<GetBookByIdQueryResponse>
    {
        public Guid Id { get; set; }

        public class Validator : AbstractValidator<GetBookByIdQuery>
        {
            public Validator()
            {
                RuleFor(p => p.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<GetBookByIdQuery, GetBookByIdQueryResponse>
        {
            private readonly CaravelTemplateDbContext _dbContext;
            private readonly IMapper _mapper;

            public Handler(CaravelTemplateDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }

            public async Task<GetBookByIdQueryResponse> Handle(GetBookByIdQuery request, CancellationToken ct)
            {
                var book = await _dbContext.Books.FindAsync(request.Id);

                if (book == null)
                    return new GetBookByIdQueryResponse.NotFound(new Error(Errors.BookNotFound,
                        $"Book {request.Id} does not exist."));

                return new GetBookByIdQueryResponse.Success(_mapper.Map<BookModel>(book));
            }
        }
    }
}