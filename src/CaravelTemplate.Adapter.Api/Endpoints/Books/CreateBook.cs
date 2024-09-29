using Caravel.AspNetCore.Endpoint;
using Caravel.AspNetCore.Http;
using Caravel.Functional;
using MediatR;

namespace CaravelTemplate.Adapter.Api.Endpoints.Books;

public class CreateBook : IEndpointFeature
{
    public void AddEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("books",
                async (ISender sender, Application.Books.CreateBook.Request command, CancellationToken ct) =>
                {
                    var result = await sender.Send(command, ct);
                    return result.Map(book => Results.Created($"/api/v1/books/{book.Id}", book),
                        err => err.ToApiProblemDetailsResult());
                })
            .WithName(nameof(CreateBook))
            .WithDescription("Get a book by it's unique identifier.")
            .WithTags(Tags.Books)
            .WithOpenApi();
    }
}