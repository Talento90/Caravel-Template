using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CaravelTemplate.Core.Interfaces.Data;
using CaravelTemplate.Entities;
using FluentValidation;
using MediatR;

namespace CaravelTemplate.Core.Books.Commands
{
    public sealed record CreateBookCommand : IRequest<CreateBookCommandResponse>
    {
        public string Name { get; init; } = null!;
        public string? Description { get; init; }

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
            private readonly ICaravelTemplateDbContext _templateDbContext;
            private readonly IMapper _mapper;
            
            public Handler(ICaravelTemplateDbContext templateDbContext, IMapper mapper)
            {
                _templateDbContext = templateDbContext;
                _mapper = mapper;
            }
            
            public async Task<CreateBookCommandResponse> Handle(CreateBookCommand request, CancellationToken ct)
            {
                var book = _mapper.Map<Book>(request);

                await _templateDbContext.Books.AddAsync(book, ct);
                await _templateDbContext.SaveChangesAsync(ct);
                
                return new CreateBookCommandResponse.Success(_mapper.Map<BookModel>(book));
            }
        }
    }
}