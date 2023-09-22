using Microsoft.EntityFrameworkCore;
using PlatformService.Modles;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app, bool IsProd)
        {
            using (var serviceScope = app.ApplicationServices.CreateAsyncScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), IsProd);
            }
        }

        private static void SeedData(AppDbContext context, bool IsProd)
        {

            if (IsProd)
            {
                Console.WriteLine("--> Attempt to apply migrations..");
                try
                {
                    context.Database.Migrate();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"--> Could not run migrations: {ex.Message}");
                }
            }
            if (!context.Platforms.Any())
            {
                Console.WriteLine("Seeding data -->");

                context.Platforms.AddRange(
                    new Platform() { Name = "Test", Publisher = "Arsho", Cost = "free" },
                    new Platform() { Name = "Test1", Publisher = "Arsho", Cost = "free" },
                    new Platform() { Name = "Test2", Publisher = "Arsho", Cost = "free" },
                    new Platform() { Name = "Test3", Publisher = "Arsho", Cost = "free" },
                    new Platform() { Name = "Test4", Publisher = "Arsho", Cost = "free" },
                    new Platform() { Name = "Test5", Publisher = "Arsho", Cost = "free" },
                    new Platform() { Name = "Test6", Publisher = "Arsho", Cost = "free" },
                    new Platform() { Name = "Test7", Publisher = "Arsho", Cost = "free" },
                    new Platform() { Name = "Test8", Publisher = "Arsho", Cost = "free" }

                );
                context.SaveChanges();

            }
            else
            {
                Console.WriteLine("--> We already have data");
            }
        }
    }
}