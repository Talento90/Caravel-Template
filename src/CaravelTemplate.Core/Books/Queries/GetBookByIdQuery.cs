using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Caravel.Exceptions;
using CaravelTemplate.Infrastructure.Data;
using FluentValidation;
using MediatR;

namespace CaravelTemplate.Core.Books.Queries
{
    public class GetBookByIdQuery : IRequest<Result<BookModel>>
    {
        public Guid Id { get; set; }
        
        public class Validator : AbstractValidator<GetBookByIdQuery>
        {
            public Validator()
            {
                RuleFor(p => p.Id).NotEmpty();
            }
        }
        
        public class Handler : IRequestHandler<GetBookByIdQuery, Result<BookModel>>
        {
            private readonly CaravelTemplateDbContext _dbContext;
            private readonly IMapper _mapper;
            
            public Handler(CaravelTemplateDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }
            
            public async Task<Result<BookModel>> Handle(GetBookByIdQuery request, CancellationToken ct)
            {
                var book = await _dbContext.Books.FindAsync(request.Id);

                if (book == null)
                    return Result<BookModel>.Create(new NotFoundException(
                        ErrorCodes.BookNotFound,
                        $"Book {request.Id} does not exist")
                    );
                
                return Result<BookModel>.Create(_mapper.Map<BookModel>(book));
            }
        }
    }
}