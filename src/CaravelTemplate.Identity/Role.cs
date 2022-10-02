using Microsoft.AspNetCore.Identity;

namespace CaravelTemplate.Identity
{
    public sealed class Role : IdentityRole<Guid>
    {
        public Role(Guid id, string name) : base(name)
        {
            Id = id;
            NormalizedName = Name.ToLower();
        }
    }
}