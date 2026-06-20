using GymManagement.BLL;
using GymManagement.BLL.Services.Classes;
using GymManagement.BLL.Services.interfaces;
using GymManagement.DAL.Data.Data_Seeding;
using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Repositories.Classes;
using GymManagement.DAL.Repositories.interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace GymCore_Project
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            //builder.Services.AddScoped<IPlanRepository, PlanRepository>();
            builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            builder.Services.AddScoped<IMemberService, MemberService>();
            builder.Services.AddScoped<IPlanService, PlanService>();
            builder.Services.AddScoped<ITrainerServicecs, TrainerService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ISessionRepository, SessionRepository>();
            builder.Services.AddScoped<ISessionService, SessionService>();
            builder.Services.AddAutoMapper(m => m.AddProfile(new MappingProfile()));
            builder.Services.AddDbContext<GymDbContext>(options=>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            var app = builder.Build();

            #region  بدل ما اقعد اكتب كل الكلام دا ممكن احطه في اكستنشن وانفظها علطول 
            //

            //using var scope = app.Services.CreateScope();

            //var dbContext = scope.ServiceProvider.GetRequiredService<GymDbContext>();
            //var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
            //var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            //if (pendingMigrations.Any())
            //{
            //    logger.LogInformation($"Applying {pendingMigrations.Count()} Pending Migrations");
            //    await dbContext.Database.MigrateAsync();
            //}
            //var seedFolderPath = Path.Combine(app.Environment.ContentRootPath, "wwwroot", "Files");
            //await GymDataSeeding.SeedAsync(dbContext, seedFolderPath, logger);  
            #endregion

           await app.MigrateAndSeedDatabaseAsync();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
