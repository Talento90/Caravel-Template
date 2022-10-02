using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CaravelTemplate.Repositories;
using FluentValidation;
using MediatR;

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
            private readonly IUnitOfWork _uow;
            private readonly IMapper _mapper;

            public Handler(IUnitOfWork uow, IMapper mapper)
            {
                _uow = uow;
                _mapper = mapper;
            }

            public async Task<GetBooksQueryResponse> Handle(GetBooksQuery request, CancellationToken ct)
            {
                var books = await _uow.BookRepository.GetBooks(request.Skip, request.Page, ct);
                return new GetBooksQueryResponse.Success(_mapper.Map<IEnumerable<BookModel>>(books));
            }
        }
    }
}