using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CaravelTemplate.Errors;
using CaravelTemplate.Repositories;
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
            private readonly IUnitOfWork _uow;
            private readonly IMapper _mapper;

            public Handler(IUnitOfWork uow, IMapper mapper)
            {
                _uow = uow;
                _mapper = mapper;
            }

            public async Task<GetBookByIdQueryResponse> Handle(GetBookByIdQuery request, CancellationToken ct)
            {
                var book = await _uow.BookRepository.GetBook(request.Id, ct);

                if (book == null)
                    return new GetBookByIdQueryResponse.NotFound(BookErrors.NotFound(request.Id));

                return new GetBookByIdQueryResponse.Success(_mapper.Map<BookModel>(book));
            }
        }
    }
}