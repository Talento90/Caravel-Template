using System;
using System.Threading;
using System.Threading.Tasks;
using Caravel.Errors;
using CaravelTemplate.Infrastructure.Data;
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
            private readonly CaravelTemplateDbContext _dbContext;

            public Handler(CaravelTemplateDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<DeleteBookCommandResponse> Handle(DeleteBookCommand request, CancellationToken ct)
            {
                var book = await _dbContext.Books.FindAsync(request.Id);

                if (book == null)
                {
                    return new DeleteBookCommandResponse.NotFound(
                        new Error(Errors.BookNotFound, $"Book {request.Id} does not exist"));
                }

                _dbContext.Remove(book);

                await _dbContext.SaveChangesAsync(ct);

                return new DeleteBookCommandResponse.Success();
            }
        }
    }
}