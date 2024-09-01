using Caravel.Functional;
using CaravelTemplate.Application.Data;
using CaravelTemplate.Application.Messaging;
using CaravelTemplate.Application.Metrics;
using CaravelTemplate.Books;
using FluentValidation;
using Mapster;
using MediatR;
using IPublisher = CaravelTemplate.Application.Messaging.IPublisher;

namespace CaravelTemplate.Application.Books;

public class CreateBook
{
    public record Response(Guid Id, string Name, string Description);

    public record Request(string Name) : IRequest<Result<Response>>
    {
        public class Validator : AbstractValidator<Request>
        {
            public Validator()
            {
                RuleFor(p => p.Name).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<Request, Result<Response>>
        {
            private readonly IUnitOfWork _unitOfWork;
            private readonly IPublisher _publisher;
            private readonly BookMetrics _metrics;

            public Handler(IUnitOfWork unitOfWork, IPublisher publisher, BookMetrics metrics)
            {
                _unitOfWork = unitOfWork;
                _publisher = publisher;
                _metrics = metrics;
            }

            public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
            {
                var book = new Book(request.Name, "Fake Description");
                var result = await _unitOfWork.BookRepository.CreateBookAsync(book, ct);

                await _unitOfWork.SaveChangesAsync(ct);

                if (result.IsSuccess)
                {
                    // TODO: Move to OutBoxPattern
                    await _publisher.PublishAsync(new BookCreatedMessage(book.Id, book), ct);
                    _metrics.IncrementBookCreated(book.Id);
                }

                return result.IsSuccess
                    ? Result<Response>.Success(book.Adapt<Response>())
                    : Result<Response>.Failure(result.Error);
            }
        }
    }
}