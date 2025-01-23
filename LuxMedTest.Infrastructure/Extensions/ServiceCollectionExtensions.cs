using LuxMedTest.Domain.Interfaces;
using LuxMedTest.Infrastructure.Data;
using LuxMedTest.Infrastructure.Repositories;
using LuxMedTest.Infrastructure.Services;
using LuxMedTest.Infrastructure.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LuxMedTest.Infrastructure.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, bool isDevelopment)
        {
            if (isDevelopment)
            {
                services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=app.db")
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Debug));
            }
            else
            {
                services.AddDbContext<AppDbContext>(options => options.UseSqlite("Data Source=app.db"));
            }

            services.AddSingleton<IPasswordHasher, PasswordHasher>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IExchangeRateService, ExchangeRateService>();

            return services;
        }
    }
}
