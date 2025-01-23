using System.Net;
using System.Text.Json;
using System.Text;
using FluentAssertions;
using LuxMedTest.Application.Dtos;
using LuxMedTest.FunctionalTests.Fixtures;

namespace LuxMedTest.FunctionalTests.ExchangeRates
{
    [Collection("DbTests")]
    public class ExchangeRatesFunctionalTests(TestFixture fixture)
    {
        private async Task<HttpClient> AuthenticateClient()
        {
            var content = new StringContent(
                JsonSerializer.Serialize(new { Username = "admin", Password = "hashed_admin123" }),
                Encoding.UTF8,
                "application/json"
            );
            var client = fixture.Factory.CreateClient();

            var response = await client.PostAsync("/api/auth/login", content);
            response.EnsureSuccessStatusCode();

            if (response.Headers.TryGetValues("Set-Cookie", out var cookieHeaders))
            {
                var cookieHeader = string.Join("; ", cookieHeaders);
                client.DefaultRequestHeaders.Add("Cookie", cookieHeader);
            }

            return client;
        }

        [Fact]
        public async Task GetExchangeRates_ShouldReturnRates_WhenCalled()
        {
            // Arrange
            var client = await AuthenticateClient();

            // Act
            var response = await client.GetAsync("/api/exchange-rates");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var responseData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var rates = JsonSerializer.Deserialize<List<ExchangeRateDto>>(responseData, options);

            rates.Should().NotBeNull();
            rates.Should().HaveCount(2);
            rates[0].Code.Should().Be("USD");
            rates[0].Rate.Should().Be(3.75m);
            rates[1].Code.Should().Be("EUR");
            rates[1].Rate.Should().Be(4.50m);
        }
    }
}
