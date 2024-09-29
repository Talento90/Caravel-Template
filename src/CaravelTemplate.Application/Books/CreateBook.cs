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
            private readonly IPublisher _publisher;

            public Handler(IUnitOfWork unitOfWork, BookMetrics metrics, IPublisher publisher)
            {
                _unitOfWork = unitOfWork;
                _metrics = metrics;
                _publisher = publisher;
            }

            public async Task<Result<Response>> Handle(Request request, CancellationToken ct)
            {
                var book = new Book(request.Name, request.Description);
                var result = await _unitOfWork.BookRepository.CreateBookAsync(book, ct);

                await _unitOfWork.SaveChangesAsync(ct);

                if (!result.IsSuccess)
                {
                    return Result<Response>.Failure(result.Error);
                    
                }

                await _publisher.PublishAsync(new BookCreatedMessage(book.Id, book), ct);
                _metrics.IncrementBookCreated(book.Id);
                return Result<Response>.Success(book.Adapt<Response>());
            }
        }
    }
}