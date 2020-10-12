using System;
using Caravel.Entities;
using Microsoft.AspNetCore.Identity;

namespace CaravelTemplate.Infrastructure.Identity
{
    public class User : IdentityUser<Guid>, IEntity, IAuditable
    {
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public Guid CreatedBy { get; set; }
        public Guid UpdatedBy { get; set; }
    }
}