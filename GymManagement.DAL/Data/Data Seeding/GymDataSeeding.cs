using GymManagement.DAL.Data.DbContexts;
using GymManagement.DAL.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace GymManagement.DAL.Data.Data_Seeding
{
    public static class GymDataSeeding
    {
        public static async Task SeedAsync(GymDbContext dbContext,string seedFolderPath,ILogger logger,CancellationToken ct=default)
        {
            try
            {
               var plans=LoadDataFromJsonFile<Plan>(seedFolderPath,"plans.json");
                if(plans.Any())
                {
                    dbContext.Plans.AddRange(plans);
                    logger.LogInformation($"Plans Seeded With Count= {plans.Count}");
                }
                if (dbContext.ChangeTracker.HasChanges())
                    await dbContext.SaveChangesAsync();
                else
                    logger.LogInformation("Plan Already Seeded");
            }
            catch (Exception ex) 
            {
                logger.LogError(ex,"Gym Data Seeding Failed");
                throw;
            
            }
        }
        private static List<T> LoadDataFromJsonFile<T>(string folderPath,string fileName)
        {
            var filepath = Path.Combine(folderPath, fileName);
            if (!File.Exists(filepath))
                throw new FileNotFoundException($"Seed Data File Not Found :{filepath}");
            var data = File.ReadAllText(filepath);
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<List<T>>(data, options) ?? [];
        }
    }
}
