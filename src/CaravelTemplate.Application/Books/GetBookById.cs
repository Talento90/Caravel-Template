using Caravel.Functional;
using CaravelTemplate.Application.Data;
using FluentValidation;
using Mapster;
using MediatR;

namespace CaravelTemplate.Application.Books;

public class GetBookById
{
    public record Response(Guid Id, string Name);

    public record Request(Guid Id) : IRequest<Result<Response>>
    {
        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(p => p.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Request, Result<Response>>
        {
            private readonly IUnitOfWork _unitOfWork;

            public Handler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            
            public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
            {
                var result = await _unitOfWork.BookRepository.GetBookAsync(request.Id, ct);
                
                return result.Map(
                    book => Result<Response>.Success(book.Adapt<Response>()),
                    Result<Response>.Failure
                );
            }
        }
    }
}