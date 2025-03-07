using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public static class SeedData
{
    public static async Task Initialize(
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Create roles if they don't exist
        string[] roleNames = { "Editor", "Writer", "Subscriber", "Guest" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
        
        // Create users if they don't exist
        if (!userManager.Users.Any())
        {
            // Create Editor
            var editor = new User
            {
                UserName = "editor@example.com",
                Email = "editor@example.com",
                Role = "Editor"
            };
            await userManager.CreateAsync(editor, "Password123!");
            await userManager.AddToRoleAsync(editor, "Editor");
            
            // Create Writer
            var writer = new User
            {
                UserName = "writer@example.com",
                Email = "writer@example.com",
                Role = "Writer"
            };
            await userManager.CreateAsync(writer, "Password123!");
            await userManager.AddToRoleAsync(writer, "Writer");
            
            // Create Subscriber
            var subscriber = new User
            {
                UserName = "subscriber@example.com",
                Email = "subscriber@example.com",
                Role = "Subscriber"
            };
            await userManager.CreateAsync(subscriber, "Password123!");
            await userManager.AddToRoleAsync(subscriber, "Subscriber");
            
            // Create sample articles
            if (!context.Articles.Any())
            {
                context.Articles.AddRange(
                    new Article
                    {
                        Title = "First News Article",
                        Content = "This is the content of the first news article.",
                        AuthorId = writer.Id,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Article
                    {
                        Title = "Second News Article",
                        Content = "This is the content of the second news article.",
                        AuthorId = writer.Id,
                        CreatedAt = DateTime.UtcNow.AddDays(-1)
                    }
                );
                await context.SaveChangesAsync();
            }
        }
    }
}