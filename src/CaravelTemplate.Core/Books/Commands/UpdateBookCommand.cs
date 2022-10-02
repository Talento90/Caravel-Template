using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CaravelTemplate.Errors;
using CaravelTemplate.Repositories;
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
            private readonly IUnitOfWork _uow;
            private readonly IMapper _mapper;
            
            public Handler(IUnitOfWork uow, IMapper mapper)
            {
                _uow = uow;
                _mapper = mapper;
            }
            
            public async Task<UpdateBookCommandResponse> Handle(UpdateBookCommand request, CancellationToken ct)
            {
                var book = await _uow.BookRepository.GetBook(request.Id, ct);

                if (book == null)
                {
                    return new UpdateBookCommandResponse.NotFound(BookErrors.NotFound(request.Id));
                }

                book.Name = request.Name ?? book.Name;
                book.Description = request.Description;

                await _uow.SaveChangesAsync(ct);

                return new UpdateBookCommandResponse.Success(
                    _mapper.Map<BookModel>(book));
            }
        }
    }
}