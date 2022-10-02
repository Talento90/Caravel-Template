using Microsoft.AspNetCore.Identity;

namespace CaravelTemplate.Identity
{
    public static class RoleSeeder
    {
        public static async Task CreateRolesAsync(RoleManager<Role> roleManager)
        {
            var roles = new [] {Roles.Admin, Roles.User};

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new Role(role));
                }
            }
        }
    }
}