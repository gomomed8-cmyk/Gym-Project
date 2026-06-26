using GymManagement.DAL.Data.Data_Seeding;
using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GymCore_Project
{
    public static class ProgramExtensions
    {
        public static async Task MigrateAndSeedDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            var userManager=scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager=scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                logger.LogInformation($"Applying {pendingMigrations.Count()} Pending Migrations");
                await dbContext.Database.MigrateAsync();
            }
            var seedFolderPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");
            await GymDataSeeding.SeedAsync(dbContext, seedFolderPath, logger);

           await IdentityDataSeeding.SeedIdentityDataAsync(roleManager, userManager, logger);
        }
    }
}
