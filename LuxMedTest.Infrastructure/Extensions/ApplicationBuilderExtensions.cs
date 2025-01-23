using LuxMedTest.Infrastructure.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LuxMedTest.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static async Task<IApplicationBuilder> UseAutoMigrationAsync(this IApplicationBuilder app)
        {
            await using (var scope = app.ApplicationServices.CreateAsyncScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                if (dbContext.Database.IsRelational())
                {
                    try
                    {
                        Console.WriteLine("Applying migrations...");
                        await dbContext.Database.MigrateAsync();
                        Console.WriteLine("Migrations applied successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error applying migrations: {ex.Message}");
                    }
                }
            }

            return app;
        }
    }
}
