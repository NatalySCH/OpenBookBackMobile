using Microsoft.AspNetCore.Identity;
using OpenBooksBackMobile.Entities;

public static class DbSeeder
{
    public static async Task SeedAdminAsync(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)
    {
        string roleName = "Administrador";
        string adminEmail = "admin@gmail.com";

        // Crear rol si no existe
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        // Crear usuario admin si no existe
        var user = await userManager.FindByEmailAsync(adminEmail);

        if (user == null)
        {
            user = new Usuario
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            await userManager.CreateAsync(user, "Admin123*");
            await userManager.AddToRoleAsync(user, roleName);
        }
    }
}