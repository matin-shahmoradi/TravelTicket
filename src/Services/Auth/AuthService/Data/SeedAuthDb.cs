using AuthService.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
    public static class SeedAuthDb
    {
        public static async Task SeedDb(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            context.Database.Migrate();

            await SeedRoles(roleManager);
            await SeedSuperAdmin(userManager);
            await SeedUser(userManager);

        }
        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            var roles = GetRoles();
            foreach (var role in roles)
            {
                if (await roleManager.FindByNameAsync(role.Name!) is null)
                {
                    var roleResult = await roleManager.CreateAsync(role);
                    if (!roleResult.Succeeded)
                        throw new Exception($"Failed to create role: {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                }
            }
        }
        private static async Task SeedSuperAdmin(UserManager<ApplicationUser> userManager)
        {
            var superAdmin = GetSuperAdmin();
            if (await userManager.FindByEmailAsync(superAdmin.Email!) is null)
            {
                var adminResult = await userManager.CreateAsync(superAdmin, "Admin12345@");
                if (!adminResult.Succeeded)
                    throw new Exception($"Failed to create user: {string.Join(", ", adminResult.Errors.Select(e => e.Description))}");

                var roleResult = await userManager.AddToRoleAsync(superAdmin, Roles.SuperAdmin);
                if (!adminResult.Succeeded)
                    throw new Exception($"Failed to  assign Super Admin role to: {string.Join(", ", adminResult.Errors.Select(e => e.Description))}");
            }
        }
        private static async Task SeedUser(UserManager<ApplicationUser> userManager)
        {
            var users = GetUsers();
            foreach (var user in users)
            {
                if (await userManager.FindByEmailAsync(user.Email!) is null)
                {
                    var userResult = await userManager.CreateAsync(user, "User12345@");
                    if (!userResult.Succeeded)
                    {
                        throw new Exception($"Failed to create user: {string.Join(", ", userResult.Errors.Select(e => e.Description))}");
                    }

                    var roleResult = await userManager.AddToRoleAsync(user, Roles.User);
                    if (!roleResult.Succeeded)
                    {
                        throw new Exception($"Failed to assign User role to '{user.Email}': {string.Join(", ", roleResult.Errors.Select(e => e.Description))}");
                    }
                }
            }
        }

        private static IEnumerable<IdentityRole> GetRoles() => new List<IdentityRole>
        {
            new IdentityRole("User"),
            new IdentityRole("Admin"),
            new IdentityRole("SuperAdmin")
        };
        private static ApplicationUser GetSuperAdmin() => new ApplicationUser
        {
            Email = "superAdmin@gmail.com",
            UserName = "superAdmin@gmail.com",
            FirstName = "matin",
            LastName = "shahmoradi"
        };
        private static IEnumerable<ApplicationUser> GetUsers() => new List<ApplicationUser>
        {
            ApplicationUser.CreateUser(
                email: "matin@gmail.com",
                username: "matin@gmail.com",
                firstname: "Matin",
                lastname: "shahmoradi",
                phoneNumber: "09999000000"
                ),

            ApplicationUser.CreateUser(
                email: "saman@gmail.com",
                username: "saman@gmail.com",
                firstname: "saman",
                lastname: "hosseiny",
                phoneNumber: "09999000000"
                ),

            ApplicationUser.CreateUser(
                email: "amin@gmail.com",
                username: "amin@gmail.com",
                firstname: "amin",
                lastname: "chahardoli",
                phoneNumber: "09999000000"
                ),

            ApplicationUser.CreateUser(
                email: "kambiz@gmail.com",
                username: "kambiz@gmail.com",
                firstname: "kambiz",
                lastname: "dirbaz",
                phoneNumber: "09999000000"
                ),

            ApplicationUser.CreateUser(
                email: "ali@gmail.com",
                username: "ali@gmail.com",
                firstname: "ali",
                lastname: "keramat",
                phoneNumber: "09999000000"
                ),
        };

    }
}
