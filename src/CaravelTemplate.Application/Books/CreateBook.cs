using Caravel.Functional;
using CaravelTemplate.Application.Data;
using CaravelTemplate.Application.Metrics;
using CaravelTemplate.Books;
using FluentValidation;
using Mapster;
using MediatR;

namespace CaravelTemplate.Application.Books;

public class CreateBook
{
    public record Response(Guid Id, string Name, string Description);

    public record Request(string Name, string Description) : IRequest<Result<Response>>
    {
        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(p => p.Name).NotEmpty();
                RuleFor(p => p.Description).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Request, Result<Response>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly BookMetrics _metrics;

            public Handler(IUnitOfWork unitOfWork, BookMetrics metrics)
            {
                _unitOfWork = unitOfWork;
                _metrics = metrics;
            }

            public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
            {
                var book = new Book(request.Name, request.Description);
                var result = await _unitOfWork.BookRepository.CreateBookAsync(book, ct);

                await _unitOfWork.SaveChangesAsync(ct);

                if (result.IsSuccess)
                {
                    _metrics.IncrementBookCreated(book.Id);
                }

                return result.IsSuccess
                    ? Result<Response>.Success(book.Adapt<Response>())
                    : Result<Response>.Failure(result.Error);
            }
        }
    }
}