using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CaravelTemplate.Identity
{
    public static class RoleSeeder
    {
        public static void SeedRoles(this ModelBuilder modelBuilder)
        {
            var roles = new [] {Identity.Roles.Admin, Identity.Roles.User};

            foreach (var role in roles)
            {
                modelBuilder.Entity<Role>().HasData(new Role(role));
            }
        }
    }
}