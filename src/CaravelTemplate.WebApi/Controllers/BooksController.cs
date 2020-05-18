using System;
using System.Threading;
using System.Threading.Tasks;
using CaravelTemplate.Core.Books;
using CaravelTemplate.Core.Books.Commands;
using CaravelTemplate.Core.Books.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CaravelTemplate.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BooksController : BaseController
    {
        private readonly IMediator _mediator;

        public BooksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}", Name = "GetBookAsync")]
        public async Task<ActionResult<BookModel>> GetBookAsync(Guid id, CancellationToken ct)
        {
            var response = await _mediator.Send(new GetBookByIdQuery {Id = id}, ct);

            return response.Match(
                success => Ok(success.Response),
                notFound => NotFound(notFound.Error));
        }

        [HttpPost]
        public async Task<ActionResult<BookModel>> CreateBookAsync(CreateBookCommand createBook, CancellationToken ct)
        {
            var response = await _mediator.Send(createBook, ct);

            return response.Match<ActionResult>(
                success => CreatedAtRoute(nameof(GetBookAsync), new {id = success.Response.Id}, success.Response)
            );
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BookModel>> UpdateBookAsync(Guid id, UpdateBookCommand updateBook,
            CancellationToken ct)
        {
            var response = await _mediator.Send(updateBook.SetId(id), ct);

            return response.Match(
                success => Ok(success.Response),
                notFound => NotFound(notFound.Error));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBookAsync(Guid id, CancellationToken ct)
        {
            var response = await _mediator.Send(new DeleteBookCommand {Id = id}, ct);

            return response.Match(
                success => NoContent(),
                notFound => NotFound(notFound.Error));
        }
    }
}