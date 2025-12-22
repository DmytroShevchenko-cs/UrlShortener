namespace UrlShortener.DAL.Database.Seed;

using Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Common.Configurations;
using Shared.Common.Constants;

public static class DatabaseSeed
{
    public static void SeedLatest(this ModelBuilder modelBuilder)
    {
        modelBuilder.SeedRoles();
    }

    private static void SeedRoles(this ModelBuilder modelBuilder)
    {
        var roles = new List<Role>
        {
            new()
            {
                Id = AuthorizationConsts.Roles.Admin.Id,
                Name = AuthorizationConsts.Roles.Admin.Name,
                NormalizedName = AuthorizationConsts.Roles.Admin.Name.ToUpper(),
                ConcurrencyStamp = "8a16f9ba-4d71-48ae-81ef-6e7f741dca47",
            },
            new Role()
            {
                Id = AuthorizationConsts.Roles.User.Id,
                Name = AuthorizationConsts.Roles.User.Name,
                NormalizedName = AuthorizationConsts.Roles.User.Name.ToUpper(),
                ConcurrencyStamp = "8a16f9ba-4d71-48ae-81ef-6e7f741dca42",
            },
        };

        modelBuilder.Entity<Role>().HasData(roles.ToArray());
    }
    
    public static async Task SeedDefaultAdminUserAsync(IServiceProvider serviceProvider, AdminConfig adminOptions)
    {
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
        var roleManager = serviceProvider.GetRequiredService<RoleManager<Role>>();
        var dbContext = serviceProvider.GetRequiredService<BaseDbContext>();
        var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();
        var logger = loggerFactory.CreateLogger("DatabaseSeed");
        
        var adminEmail = adminOptions.DefaultAdminUserEmail;
        var adminPassword = adminOptions.DefaultAdminUserPassword;
        var adminFirstName = adminOptions.DefaultAdminUserFirstName;
        var adminLastName = adminOptions.DefaultAdminUserLastName;

        if (string.IsNullOrWhiteSpace(adminEmail) || string.IsNullOrWhiteSpace(adminPassword) ||
            string.IsNullOrWhiteSpace(adminFirstName) || string.IsNullOrWhiteSpace(adminLastName))
        {
            logger.LogWarning("Default SA user creation skipped: Missing required configuration values.");
            return;
        }

        var existingUser = await userManager.FindByEmailAsync(adminEmail);
        if (existingUser != null)
        {
            return;
        }

        
        var adminRole = await roleManager.FindByIdAsync(AuthorizationConsts.Roles.Admin.Id.ToString());
        if (adminRole == null)
        {
            adminRole = await roleManager.FindByNameAsync(AuthorizationConsts.Roles.Admin.Name);
            
            if (adminRole == null)
            {
                adminRole = new Role()
                {
                    Id = AuthorizationConsts.Roles.Admin.Id,
                    Name = AuthorizationConsts.Roles.Admin.Name,
                    NormalizedName = AuthorizationConsts.Roles.Admin.Name.ToUpper(),
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                };
                
                dbContext.Set<Role>().Add(adminRole);
                await dbContext.SaveChangesAsync();
                logger.LogInformation("Admin role created successfully.");
            }
        }

        var adminUser = new User
        {
            FirstName = adminFirstName,
            LastName = adminLastName,
            Email = adminEmail,
            UserName = adminEmail,
            EmailConfirmed = true,
            CreatedAt = DateTimeOffset.UtcNow
        };

        var createResult = await userManager.CreateAsync(adminUser, adminPassword);
        if (!createResult.Succeeded)
        {
            var errors = string.Join(", ", createResult.Errors.Select(e => $"{e.Code}: {e.Description}"));
            throw new InvalidOperationException(
                $"Failed to create default admin user: {errors}");
        }

        var addRoleResult = await userManager.AddToRoleAsync(adminUser, adminRole.Name!);
        if (!addRoleResult.Succeeded)
        {
            var errors = string.Join(", ", addRoleResult.Errors.Select(e => $"{e.Code}: {e.Description}"));
            throw new InvalidOperationException(
                $"Failed to add SA role to default admin user: {errors}");
        }
    }
}