using System;
using System.Threading;
using System.Threading.Tasks;
using Caravel.Errors;
using CaravelTemplate.Repositories;
using FluentValidation;
using MediatR;

namespace CaravelTemplate.Core.Books.Commands
{
    public class DeleteBookCommand : IRequest<DeleteBookCommandResponse>
    {
        public Guid Id { get; set; }

        public class Validator : AbstractValidator<DeleteBookCommand>
        {
            public Validator()
            {
                RuleFor(p => p.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<DeleteBookCommand, DeleteBookCommandResponse>
        {
            private readonly IBookRepository _bookRepository;

            public Handler(IBookRepository bookRepository)
            {
                _bookRepository = bookRepository;
            }

            public async Task<DeleteBookCommandResponse> Handle(DeleteBookCommand request, CancellationToken ct)
            {
                var book = await _bookRepository.GetAsync(request.Id, ct);

                if (book == null)
                {
                    return new DeleteBookCommandResponse.NotFound(
                        new Error(Errors.BookNotFound, $"Book {request.Id} does not exist"));
                }

                _bookRepository.DeleteAsync(book);

                await _bookRepository.UnitOfWork.SaveChangesAsync(ct);

                return new DeleteBookCommandResponse.Success();
            }
        }
    }
}