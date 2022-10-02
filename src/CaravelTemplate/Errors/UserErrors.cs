using System;
using Caravel.Errors;

namespace CaravelTemplate.Errors;

public static class UserErrors
{
    public static Error NotFound(Guid id) => new ("user_not_found", $"User {id} does not exist.");
    public static Error NotFound(string username) => new ("user_not_found", $"User {username} does not exist.");
    public static Error Create() => new ("user_creation", $"Error creating user.");
    
    public static Error PasswordMismatch() => new ("password_mismatch", $"Passwords don't match.");
}