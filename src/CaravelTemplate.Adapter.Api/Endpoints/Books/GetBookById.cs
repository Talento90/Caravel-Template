using Caravel.AspNetCore.Endpoint;
using Caravel.AspNetCore.Http;
using Caravel.Functional;
using MediatR;

namespace CaravelTemplate.Adapter.Api.Endpoints.Books;

public class GetBookById : IEndpointFeature
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("books/{bookId}", async (Guid bookId, ISender sender, CancellationToken ct) =>
            {
                var query = new Application.Books.GetBookById.Request(bookId);
                var result = await sender.Send(query, ct);
                return result.Map(Results.Ok, err => err.ToApiProblemDetailsResult());
            })
            .WithName(nameof(GetBookById))
            .WithDescription("Get a book by it's unique identifier.")
            .WithTags(Tags.Books)
            .WithOpenApi();
    }
}