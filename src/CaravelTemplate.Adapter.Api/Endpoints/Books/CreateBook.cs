using Caravel.AspNetCore.Http;
using Caravel.Functional;
using MediatR;

namespace CaravelTemplate.Adapter.Api.Endpoints.Books;

public class CreateBook : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("books", async (ISender sender, CancellationToken ct) =>
            {
                var command = new Application.Books.CreateBook.Request("Test Book");
                var result = await sender.Send(command, ct);
                return result.Map(book => Results.Created("", book), err => err.ToHttpResult());
            })
            .WithName(nameof(CreateBook))
            .WithDescription("Get a book by it's unique identifier.")
            //.HasPermission(Permissions.Books.Write)
            .WithTags(Tags.Books)
            .WithOpenApi();
    }
}