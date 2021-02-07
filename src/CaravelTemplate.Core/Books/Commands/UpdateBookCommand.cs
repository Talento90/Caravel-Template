using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Caravel.Errors;
using CaravelTemplate.Core.Interfaces.Data;
using FluentValidation;
using MediatR;

namespace CaravelTemplate.Core.Books.Commands
{
    public sealed record  UpdateBookCommand : IRequest<UpdateBookCommandResponse>
    {
        private Guid Id { get; set; }
        public string? Name { get; init; }
        public string? Description { get; init; }

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
            private readonly ICaravelTemplateDbContext _templateDbContext;
            private readonly IMapper _mapper;
            
            public Handler(ICaravelTemplateDbContext templateDbContext, IMapper mapper)
            {
                _templateDbContext = templateDbContext;
                _mapper = mapper;
            }
            
            public async Task<UpdateBookCommandResponse> Handle(UpdateBookCommand request, CancellationToken ct)
            {
                var book = await _templateDbContext.Books.FindAsync(request.Id);

                if (book == null)
                {
                    return new UpdateBookCommandResponse.NotFound(
                        new Error(Errors.BookNotFound, $"Book {request.Id} does not exist")
                    );
                }

                book.Name = request.Name ?? book.Name;
                book.Description = request.Description;
                
                await _templateDbContext.SaveChangesAsync(ct);

                return new UpdateBookCommandResponse.Success(
                    _mapper.Map<BookModel>(book));
            }
        }
    }
}