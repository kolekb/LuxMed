using LuxMedTest.Domain.Models;
using LuxMedTest.Infrastructure.Clients.Models;
using System.Net;
using System.Text;
using System.Text.Json;

namespace LuxMedTest.FunctionalTests.Utils
{
    public class MockHttpMessageHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var mockData = new List<ExchangeRateTable>
            {
                {
                    new ExchangeRateTable
                    {
                        Table = "A",
                        No = "015/A/NBP/2025",
                        EffectiveDate = "2025-01-22",
                        Rates = new List<ExchangeRateEntry>
                        {
                            new ExchangeRateEntry { Currency = "US Dollar", Code = "USD", Mid = 3.75m },
                            new ExchangeRateEntry { Currency = "Euro", Code = "EUR", Mid = 4.50m }
                        }
                    }
                }
            };
            var jsonResponse = JsonSerializer.Serialize(mockData);

            return Task.FromResult(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse, Encoding.UTF8, "application/json")
            });
        }
    }
}
