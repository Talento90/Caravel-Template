using Caravel.Exceptions;

namespace CaravelTemplate
{
    public static class ErrorCodes
    {
        // Not Found Errors
        public static readonly Error BookNotFound = new Error(30001, "Book does not exist");
    }
}