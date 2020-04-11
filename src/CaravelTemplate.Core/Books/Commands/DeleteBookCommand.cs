using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Caravel.Exceptions;
using CaravelTemplate.Entities;
using CaravelTemplate.Infrastructure.Data;
using FluentValidation;
using MediatR;

namespace CaravelTemplate.Core.Books.Commands
{
    public class DeleteBookCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }

        public class Validator : AbstractValidator<DeleteBookCommand>
        {
            public Validator()
            {
                RuleFor(p => p.Id).NotEmpty();
            }
        }

        public class Handler : IRequestHandler<DeleteBookCommand, Result<bool>>
        {
            private readonly CaravelTemplateDbContext _dbContext;

            public Handler(CaravelTemplateDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public async Task<Result<bool>> Handle(DeleteBookCommand request, CancellationToken ct)
            {
                var book = await _dbContext.Books.FindAsync(request.Id);

                if (book == null)
                {
                    return Result<bool>.Create(new NotFoundException(
                        ErrorCodes.BookNotFound,
                        $"Book {request.Id} does not exist")
                    );
                }

                _dbContext.Books.Remove(book);

                await _dbContext.SaveChangesAsync(ct);

                return Result<bool>.Create(true);
            }
        }
    }
}