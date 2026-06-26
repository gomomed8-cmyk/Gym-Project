using GymManagement.DAL.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Data.Data_Seeding
{
    public class IdentityDataSeeding
    {
        public static async Task SeedIdentityDataAsync(RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            ILogger logger,
            CancellationToken ct=default)
        {
            try
            {

            bool hasUsers= await userManager.Users.AnyAsync(ct);
            bool hasRoles= await roleManager.Roles.AnyAsync(ct);
            if (hasUsers && hasRoles) return;
            if(!hasRoles)
            {
                var roles = new List<IdentityRole>()
                {
                    new IdentityRole("SuperAdmin"),
                    new IdentityRole("Admin")
                };
                foreach (var role in roles)
                {
                    if(!await roleManager.RoleExistsAsync(role.Name!))

                    {
                        var roleResult = await roleManager.CreateAsync(role);
                        if (!roleResult.Succeeded)
                        {
                            logger.LogError($"Failed To Create Role{role.Name} : {string.Join(" ; ",roleResult.Errors.Select(ex=>ex.Description))}");

                        }
                    }
                 
                }
            }
            if(!hasUsers)
            {
                var mainAdmin = new ApplicationUser()
                {
                    FirstName = "Yuosef",
                    LastName = "Mohamed",
                    Email = "yuosefmohamed@gmail.com",
                    UserName ="YuosefMohamed",
                    PhoneNumber = "01068455824"

                };
              var userResult = await userManager.CreateAsync(mainAdmin,"P@ssw0rd");
                if (!userResult.Succeeded)
                {
                    logger.LogError($"Failed To Create Admin{mainAdmin.UserName}");
                    return;
                }

                  var roleResult= await userManager.AddToRoleAsync(mainAdmin, "SuperAdmin");
                if (!roleResult.Succeeded)
                {
                    logger.LogError($"Failed To Add {mainAdmin.UserName} To SuperAdmin Role");
                    return; 
                }
              
                
                var Admin = new ApplicationUser()
                {
                    FirstName = "goks",
                    LastName = "mo7amed",
                    Email = "goksmo7amed@gmail.com",
                    UserName = "goksmoh7amed",
                    PhoneNumber = "01068455830"
                };
                    await userManager.CreateAsync(Admin, "P@ssw0rd");
                    await userManager.AddToRoleAsync(Admin, "Admin");
                logger.LogInformation($"Identity Data Seeded");

            }
            return;
            }
            catch (Exception ex)
            {
                logger.LogError(ex,"Identity Seeding Failed");
                return;
            }

        }
    }
}
