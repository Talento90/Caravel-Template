using System;
using System.Threading;
using System.Threading.Tasks;
using CaravelTemplate.Errors;
using CaravelTemplate.Repositories;
using FluentValidation;
using MediatR;

namespace CaravelTemplate.Core.Books.Commands
{
    public sealed record DeleteBookCommand : IRequest<DeleteBookCommandResponse>
    {
        public Guid Id { get; init; }

        public class Validator : AbstractValidator<DeleteBookCommand>
        {
            public Validator()
            {
                RuleFor(p => p.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<DeleteBookCommand, DeleteBookCommandResponse>
        {
            private readonly IUnitOfWork _uow;

            public Handler(IUnitOfWork uow)
            {
                _uow = uow;
            }

            public async Task<DeleteBookCommandResponse> Handle(DeleteBookCommand request, CancellationToken ct)
            {
                var book = await _uow.BookRepository.GetBook(request.Id, ct);

                if (book == null)
                {
                    return new DeleteBookCommandResponse.NotFound(BookErrors.NotFound(request.Id));
                }

                await _uow.BookRepository.DeleteBook(book, ct);
                await _uow.SaveChangesAsync(ct);

                return new DeleteBookCommandResponse.Success();
            }
        }
    }
}