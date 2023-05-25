using ChatApp.Business.Helpers;
using ChatApp.Context.EntityClasses;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

public static class IdentityInitializer
{
    public static async Task InitializeAsync(UserManager<Profile> userManager, RoleManager<IdentityRole> roleManager)
    {
        string adminRole = "Admin";
        string adminEmail = "chatappadmin@gmail.com";
        string adminPassword = "Admin123*";
        string firstName = "ChatApp";
        string lastName = "Admin";
        string userName = "chatappadmin";
        int designationID = 9;

        // Create the admin role if it doesn't exist
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        // Create the admin user if it doesn't exist
        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new Profile
            {
                UserName = userName,
                Email = adminEmail,
                FirstName = firstName,
                LastName = lastName,
                DesignationID = designationID,
                StatusID = 1,
                ProfileType = ProfileType.Administrator
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                // Assign the admin user to the admin role
                await userManager.AddToRoleAsync(adminUser, adminRole);
            }
        }
    }
}