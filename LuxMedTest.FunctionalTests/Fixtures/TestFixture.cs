using LuxMedTest.Domain.Interfaces;
using LuxMedTest.Domain.Models;
using LuxMedTest.FunctionalTests.Utils;
using LuxMedTest.Infrastructure.Data;
using LuxMedTest.Infrastructure.Services;
using LuxMedTest.Infrastructure.Utils;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;

namespace LuxMedTest.FunctionalTests.Fixtures
{
    public class TestFixture : IDisposable
    {
        public WebApplicationFactory<Program> Factory { get;}

        public TestFixture()
        {
            Factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    // Usunięcie istniejącego kontekstu bazy danych
                    var descriptors = services.Where(
                        d => d.ServiceType.FullName != null &&
                             d.ServiceType.FullName.Contains("DbContextOptions")
                    ).ToList();
                    foreach (var descriptor in descriptors)
                    {
                        services.Remove(descriptor);
                    }

                    // Dodanie bazy InMemory
                    services.AddDbContext<AppDbContext>(options =>
                        options.UseInMemoryDatabase("TestDb")
                    );

                    // Mockowanie HttpClient w ExchangeRateService
                    // W przypadku wiekszej ilosci testow utworzyc oddzielne fixture dla klas testow funkcjonalnych
                    var mockHttpMessageHandler = new MockHttpMessageHandler();
                    var httpClient = new HttpClient(mockHttpMessageHandler)
                    {
                        BaseAddress = new Uri("http://mocked-nbp-api")
                    };
                    var mockLogger = Mock.Of<ILogger<ExchangeRateService>>();

                    services.AddSingleton<IExchangeRateService>(new ExchangeRateService(httpClient, mockLogger));
                });


            });
            var scope = Factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            InitializeDatabase(context);
        }

        private void InitializeDatabase(AppDbContext appDbContext)
        {
            ClearDatabase(appDbContext);
            SeedTestData(appDbContext);
        }

        private void ClearDatabase(AppDbContext appDbContext)
        {
            appDbContext.Database.EnsureDeleted();
            appDbContext.Database.EnsureCreated();
        }

        private void SeedTestData(AppDbContext appDbContext)
        {
            var passwordHasher = new PasswordHasher();
            appDbContext.Users.Add(new User
            {
                Id = 1,
                Username = "admin",
                Password = passwordHasher.HashPassword("hashed_admin123")
            });
            appDbContext.SaveChanges();
        }

        public void Dispose()
        {
            Factory.Dispose();
        }
    }
}