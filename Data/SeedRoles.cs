using Microsoft.AspNetCore.Identity;

namespace Ep_Linares.Data
{
    public static class SeedRoles
    {
        private static readonly string[] Roles = new[] { "Coordinador" };

        public static async Task InitializeAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}
