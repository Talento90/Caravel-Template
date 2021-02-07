using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Caravel.Errors;
using CaravelTemplate.Core.Interfaces.Data;
using FluentValidation;
using MediatR;

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
            private readonly ICaravelTemplateDbContext _templateDbContext;
            private readonly IMapper _mapper;

            public Handler(ICaravelTemplateDbContext templateDbContext, IMapper mapper)
            {
                _templateDbContext = templateDbContext;
                _mapper = mapper;
            }

            public async Task<GetBookByIdQueryResponse> Handle(GetBookByIdQuery request, CancellationToken ct)
            {
                var book = await _templateDbContext.Books.FindAsync(request.Id);

                if (book == null)
                    return new GetBookByIdQueryResponse.NotFound(new Error(Errors.BookNotFound,
                        $"Book {request.Id} does not exist."));

                return new GetBookByIdQueryResponse.Success(_mapper.Map<BookModel>(book));
            }
        }
    }
}