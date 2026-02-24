using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using fullcalendarcore.Models;

namespace fullcalendarcore.DataAccessLayer
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Create Admin role if it doesn't exist
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Create User role if it doesn't exist
            if (!await roleManager.RoleExistsAsync("User"))
            {
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            // Create default admin user if no users exist
            if (userManager.Users.Any() == false)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    DisplayName = "Administrator",
                    Email = "admin@wardenaar.org",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                    Console.WriteLine("Default admin user created:");
                    Console.WriteLine("Username: admin");
                    Console.WriteLine("Password: Admin123!");
                }
            }
        }
    }
}
