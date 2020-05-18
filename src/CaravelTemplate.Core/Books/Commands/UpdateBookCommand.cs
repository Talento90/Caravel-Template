using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Caravel.Errors;
using CaravelTemplate.Repositories;
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
            private readonly IBookRepository _bookRepository;
            private readonly IMapper _mapper;
            
            public Handler(IBookRepository bookRepository, IMapper mapper)
            {
                _bookRepository = bookRepository;
                _mapper = mapper;
            }
            
            public async Task<UpdateBookCommandResponse> Handle(UpdateBookCommand request, CancellationToken ct)
            {
                var book = await _bookRepository.GetAsync(request.Id, ct);

                if (book == null)
                {
                    return new UpdateBookCommandResponse.NotFound(
                        new Error(Errors.BookNotFound, $"Book {request.Id} does not exist")
                    );
                }

                book.Name = request.Name ?? book.Name;
                book.Description = request.Description;
                
                _bookRepository.UpdateAsync(book);
                
                await _bookRepository.UnitOfWork.SaveChangesAsync(ct);

                return new UpdateBookCommandResponse.Success(
                    _mapper.Map<BookModel>(book));
            }
        }
    }
}