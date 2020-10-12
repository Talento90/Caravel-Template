using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Caravel.Errors;
using CaravelTemplate.Infrastructure.Data;
using FluentValidation;
using MediatR;

namespace CaravelTemplate.Core.Books.Commands
{
    public class UpdateBookCommand : IRequest<UpdateBookCommandResponse>
    {
        private Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

        public UpdateBookCommand SetId(Guid id)
        {
            Id = id;
            return this;
        }

        public class Validator : AbstractValidator<UpdateBookCommand>
        {
            public Validator()
            {
                RuleFor(p => p.Name).MaximumLength(50);
                RuleFor(p => p.Description).MaximumLength(100);
            }
        }
        
        public class Handler : IRequestHandler<UpdateBookCommand, UpdateBookCommandResponse>
        {
            private readonly CaravelTemplateDbContext _dbContext;
            private readonly IMapper _mapper;
            
            public Handler(CaravelTemplateDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }
            
            public async Task<UpdateBookCommandResponse> Handle(UpdateBookCommand request, CancellationToken ct)
            {
                var book = await _dbContext.Books.FindAsync(request.Id);

                if (book == null)
                {
                    return new UpdateBookCommandResponse.NotFound(
                        new Error(Errors.BookNotFound, $"Book {request.Id} does not exist")
                    );
                }

                book.Name = request.Name ?? book.Name;
                book.Description = request.Description;
                
                await _dbContext.SaveChangesAsync(ct);

                return new UpdateBookCommandResponse.Success(
                    _mapper.Map<BookModel>(book));
            }
        }
    }
}