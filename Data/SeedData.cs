using Microsoft.AspNetCore.Identity;
using BackTodoApi.Data;
using BackTodoApi.Models;

public static class SeedData
{
    public static async Task SeedAsync(TodoContext db, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        var roles = new[] { "Admin", "User" };
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }

        var adminEmail = "admin@example.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);
        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                EmailConfirmed = true
            };

            var res = await userManager.CreateAsync(adminUser, "Admin123!");
            if (!res.Succeeded)
                throw new Exception($"Failed to create admin user: {string.Join(", ", res.Errors.Select(e => e.Description))}");

            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        var userEmail = "user@example.com";
        var regularUser = await userManager.FindByEmailAsync(userEmail);
        if (regularUser == null)
        {
            regularUser = new ApplicationUser
            {
                UserName = "user",
                Email = userEmail,
                EmailConfirmed = true
            };

            var res = await userManager.CreateAsync(regularUser, "User123!");
            if (!res.Succeeded)
                throw new Exception($"Failed to create regular user: {string.Join(", ", res.Errors.Select(e => e.Description))}");

            await userManager.AddToRoleAsync(regularUser, "User");
        }

        if (!db.Todos.Any(t => t.UserId == regularUser.Id))
        {
            var todos = new[]
            {
                new Todo { Title = "Seeded Todo 1", Description = "Created by SeedData", IsCompleted = false, UserId = regularUser.Id },
                new Todo { Title = "Seeded Todo 2", Description = "Another seeded task", IsCompleted = false, UserId = regularUser.Id }
            };

            db.Todos.AddRange(todos);
            await db.SaveChangesAsync();
        }
    }
}
