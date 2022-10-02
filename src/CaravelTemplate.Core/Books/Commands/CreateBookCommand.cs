using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CaravelTemplate.Entities;
using CaravelTemplate.Repositories;
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
            private readonly IUnitOfWork _uow;
            private readonly IMapper _mapper;
            
            public Handler(IUnitOfWork uow, IMapper mapper)
            {
                _uow = uow;
                _mapper = mapper;
            }
            
            public async Task<CreateBookCommandResponse> Handle(CreateBookCommand request, CancellationToken ct)
            {
                var book = _mapper.Map<Book>(request);

                await _uow.BookRepository.CreateBook(book, ct);
                await _uow.SaveChangesAsync(ct);
                
                return new CreateBookCommandResponse.Success(_mapper.Map<BookModel>(book));
            }
        }
    }
}