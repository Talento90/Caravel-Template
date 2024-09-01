using Caravel.Errors;

namespace CaravelTemplate.Users;

public static class UserErrors
{
    public const string NotFoundCode = "user_not_found";
    public const string UserCreationCode = "user_creation";
    public const string PasswordMismatchCode = "password_mismatch";

    public static Error UserNotFound(Guid id) => Error.NotFound(NotFoundCode, $"User {id} does not exist.");
    public static Error UsernameNotFound(string username) => Error.NotFound(NotFoundCode, $"User {username} does not exist.");
    public static Error Create() => Error.Validation(UserCreationCode, $"Error creating user.");
    public static Error PasswordMismatch() => Error.Validation(PasswordMismatchCode, $"Passwords don't match.");
}