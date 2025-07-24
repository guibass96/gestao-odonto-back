using GBarber.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace GBarber.Infrastructure.Seeds
{
    public class DefaultRoles
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Enum.GetValues(typeof(Roles)))
            {
                var roleName = role.ToString();
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

    }
}
