using System;
using System.Threading;
using System.Threading.Tasks;
using Caravel.Errors;
using CaravelTemplate.Core.Data;
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
            private readonly ICaravelTemplateDbContext _templateDbContext;

            public Handler(ICaravelTemplateDbContext templateDbContext)
            {
                _templateDbContext = templateDbContext;
            }

            public async Task<DeleteBookCommandResponse> Handle(DeleteBookCommand request, CancellationToken ct)
            {
                var book = await _templateDbContext.Books.FindAsync(request.Id);

                if (book == null)
                {
                    return new DeleteBookCommandResponse.NotFound(
                        new Error(Errors.BookNotFound, $"Book {request.Id} does not exist"));
                }

                _templateDbContext.Books.Remove(book);

                await _templateDbContext.SaveChangesAsync(ct);

                return new DeleteBookCommandResponse.Success();
            }
        }
    }
}