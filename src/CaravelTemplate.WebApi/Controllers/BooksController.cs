using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Caravel.AspNetCore.Http;
using Caravel.Exceptions;
using CaravelTemplate.Core.Books;
using CaravelTemplate.Core.Books.Commands;
using CaravelTemplate.Core.Books.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CaravelTemplate.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BooksController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BooksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}", Name = "GetBookAsync")]
        public async Task<ActionResult<BookModel>> GetBookAsync(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetBookByIdQuery {Id = id}, ct);

            if (result.Exception is NotFoundException)
            {
                return NotFound(new HttpError(HttpContext, HttpStatusCode.NotFound, result.Exception));
            }

            return Ok(result.Data);
        }

        [HttpPost]
        public async Task<ActionResult<BookModel>> CreateBookAsync(CreateBookCommand createBook, CancellationToken ct)
        {
            var result = await _mediator.Send(createBook, ct);
            
            return CreatedAtRoute(nameof(GetBookAsync), new { id = result.Data.Id }, result.Data);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<BookModel>> UpdateBookAsync(Guid id, UpdateBookCommand updateBook,
            CancellationToken ct)
        {
            var result = await _mediator.Send(updateBook.SetId(id), ct);

            if (result.Exception is NotFoundException)
            {
                return NotFound(new HttpError(HttpContext, HttpStatusCode.NotFound, result.Exception));
            }

            return Ok(result.Data);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteBookAsync(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeleteBookCommand {Id = id}, ct);

            if (result.Exception is NotFoundException)
            {
                return NotFound(new HttpError(HttpContext, HttpStatusCode.NotFound, result.Exception));
            }

            return NoContent();
        }
    }
}