using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CaravelTemplate.Entities;
using CaravelTemplate.Infrastructure.Data;
using FluentValidation;
using MediatR;

namespace CaravelTemplate.Core.Books.Commands
{
    public class CreateBookCommand : IRequest<CreateBookCommandResponse>
    {
        public string Name { get; set; } = null!;
        public string? Description { get; set; }

        public class Validator : AbstractValidator<CreateBookCommand>
        {
            public Validator()
            {
                RuleFor(p => p.Name)
                    .NotEmpty()
                    .MaximumLength(50);
                RuleFor(p => p.Description).MaximumLength(100);
            }
        }
        
        public class Handler : IRequestHandler<CreateBookCommand, CreateBookCommandResponse>
        {
            private readonly CaravelTemplateDbContext _dbContext;
            private readonly IMapper _mapper;
            
            public Handler(CaravelTemplateDbContext dbContext, IMapper mapper)
            {
                _dbContext = dbContext;
                _mapper = mapper;
            }
            
            public async Task<CreateBookCommandResponse> Handle(CreateBookCommand request, CancellationToken ct)
            {
                var book = _mapper.Map<Book>(request);

                await _dbContext.Books.AddAsync(book, ct);

                await _dbContext.SaveChangesAsync(ct);
                
                return new CreateBookCommandResponse.Success(_mapper.Map<BookModel>(book));
            }
        }
    }
}