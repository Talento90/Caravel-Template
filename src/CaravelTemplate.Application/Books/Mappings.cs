using CaravelTemplate.Application.Books;
using CaravelTemplate.Books;
using Mapster;

namespace CaravelTemplate.Application.Mappings;

public static class Mappings
{
    public static void Map()
    {
        TypeAdapterConfig<Book, GetBookById.Request>.NewConfig()
            .Map(dest => dest.Id, src => src.Id); // Example how to map custom properties.
    }
}