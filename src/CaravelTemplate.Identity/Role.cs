using Microsoft.AspNetCore.Identity;

namespace CaravelTemplate.Identity
{
    public class Role : IdentityRole<Guid>
    {
        public Role(string name) : base(name)
        {
            
        }
    }
}