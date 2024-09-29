using Caravel.Errors;

namespace CaravelTemplate.Books;

public static class BookErrors
{
    public const string NotFoundCode = "book_not_found"; 
    
    public static Error NotFound(Guid id) => Error.NotFound(NotFoundCode, $"Book {id} does not exist.");
}