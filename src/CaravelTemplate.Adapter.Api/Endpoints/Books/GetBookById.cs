using Caravel.AspNetCore.Http;
using Caravel.Functional;
using CaravelTemplate.Adapter.Api.Extensions;
using MediatR;

namespace CaravelTemplate.Adapter.Api.Endpoints.Books;

public class GetBookById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("books/{bookId}", async (Guid bookId, ISender sender, CancellationToken ct) =>
            {
                var query = new Application.Books.GetBookById.Request(bookId);
                var result = await sender.Send(query, ct);
                return result.Map(Results.Ok, err => err.ToHttpResult());
            })
            .WithName(nameof(GetBookById))
            .WithDescription("Get a book by it's unique identifier.")
            //.HasPermission(Permissions.Books.Read)
            .WithTags(Tags.Books)
            .WithOpenApi();
    }
}