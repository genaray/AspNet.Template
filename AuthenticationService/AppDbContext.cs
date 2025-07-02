using AuthenticationService.Feature.UserCredentials;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationService;

public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<UserCredentials>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
    
    /// <summary>
    ///     Seed users and roles in the Identity database.
    /// </summary>
    /// <param name="userManager">ASP.NET Core Identity User Manager</param>
    /// <param name="roleManager">ASP.NET Core Identity Role Manager</param>
    /// <returns></returns>
    public static async Task SeedAsync(UserManager<UserCredentials> userManager, RoleManager<IdentityRole> roleManager)
    {
        if (!await roleManager.RoleExistsAsync(Shared.Roles.ADMIN.ToString()))
        {
            await roleManager.CreateAsync(new IdentityRole(Shared.Roles.ADMIN.ToString()));
        }

        if (!await roleManager.RoleExistsAsync(Shared.Roles.USER.ToString()))
        {
            await roleManager.CreateAsync(new IdentityRole(Shared.Roles.USER.ToString()));
        }
        
        if (await userManager.FindByNameAsync("Admin") == null)
        {
            var adminUser = new UserCredentials
            {
                UserName = "Admin",
                NormalizedUserName = "Admin".ToUpper(),
                Email = "admin@example.com",
                NormalizedEmail = "admin@example.com".ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
                RegisterDate = DateTime.UtcNow,
                LastLoginDate = DateTime.UtcNow,
            };

            var result = await userManager.CreateAsync(adminUser, "AdminPassword123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, Shared.Roles.ADMIN.ToString());
            }
        }
    }
}
