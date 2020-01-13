using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Armaiti.Identity.Data
{
    public static class RoleSeed
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string Member = "Member";

        /// <summary>
        ///     Adding power user to Admin role
        /// </summary>
        /// <param name="serviceProvider">
        ///     Defines a mechanism for retrieving a service object; that is, an object that provides custom support to other objects.
        /// </param>
        /// <param name="configuration">
        ///     Represents a set of key/value application configuration properties.
        /// </param>
        /// <returns></returns>
        public static async Task AddPowerUserAsync(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            // creating customs roles if not exist
            await CreateRolesAsync(serviceProvider);

            var userManager = serviceProvider.GetRequiredService<UserManager<ArmaitiUser>>();
            var powerUser = await userManager.FindByEmailAsync(configuration.GetSection("PowerUser")["email"]);
            if (powerUser != null && !(await userManager.IsInRoleAsync(powerUser, Admin)))
            {
                await userManager.AddToRoleAsync(powerUser, Admin);
            }
        }

        /// <summary>
        ///     Adding customs roles (Admin, Manager, Member)
        /// </summary>
        /// <param name="serviceProvider">
        ///     Defines a mechanism for retrieving a service object; that is, an object that provides custom support to other objects.
        /// </param>
        /// <returns></returns>
        public static async Task CreateRolesAsync(IServiceProvider serviceProvider)
        {
            //
            string[] roleNames = { Admin, Manager, Member };
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            foreach (var roleName in roleNames)
            {
                // creating the roles and seeding them to the database
                if (!(await roleManager.RoleExistsAsync(roleName)))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
