using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CaravelTemplate.Entities;
using CaravelTemplate.Repositories;
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
            private readonly IBookRepository _bookRepository;
            private readonly IMapper _mapper;
            
            public Handler(IBookRepository bookRepository, IMapper mapper)
            {
                _bookRepository = bookRepository;
                _mapper = mapper;
            }
            
            public async Task<CreateBookCommandResponse> Handle(CreateBookCommand request, CancellationToken ct)
            {
                var book = _mapper.Map<Book>(request);

                await _bookRepository.CreateAsync(book, ct);

                await _bookRepository.UnitOfWork.SaveChangesAsync(ct);
                
                return new CreateBookCommandResponse.Success(_mapper.Map<BookModel>(book));
            }
        }
    }
}