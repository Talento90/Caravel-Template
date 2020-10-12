using System;
using Microsoft.AspNetCore.Identity;

namespace CaravelTemplate.Infrastructure.Identity
{
    public class Role : IdentityRole<Guid>
    {
        public Role(string name) : base(name)
        {
            
        }
    }
}