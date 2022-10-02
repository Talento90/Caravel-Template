using Microsoft.EntityFrameworkCore;

namespace CaravelTemplate.Identity
{
    public static class RoleSeeder
    {
        public static void SeedRoles(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Role>().HasData(new Role(Guid.Parse("A747465C-F1E5-48BC-9ABE-35D3864C5121"), Roles.Admin));
            modelBuilder.Entity<Role>().HasData(new Role(Guid.Parse("D433C4EE-E3BA-4C32-83F0-F4283D2D14F7"), Roles.User));
        }
    }
}