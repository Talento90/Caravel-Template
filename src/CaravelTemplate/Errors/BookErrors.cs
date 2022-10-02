using System;
using Caravel.Errors;

namespace CaravelTemplate.Errors;

public static class BookErrors
{
    public static Error NotFound(Guid id) => new Error("book_not_found", $"Book {id} does not exist.");
}