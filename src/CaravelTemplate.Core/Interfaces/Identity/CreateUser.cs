using System.Collections.Generic;

namespace CaravelTemplate.Core.Interfaces.Identity
{
    public record CreateUser(
        string Username,
        string FirstName,
        string LastName,
        string Email,
        string Password,
        IEnumerable<string> Roles);
}