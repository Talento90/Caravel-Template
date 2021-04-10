using System.Threading.Tasks;
using CaravelTemplate.Core.Interfaces.Identity;
using CaravelTemplate.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace CaravelTemplate.Infrastructure.Data
{
    public static class SeedDatabase
    {
        public static async Task SeedRoles(RoleManager<Role> roleManager)
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