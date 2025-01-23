using LuxMedTest.Domain.Interfaces;
using LuxMedTest.Domain.Models;
using LuxMedTest.Infrastructure.Clients.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LuxMedTest.Infrastructure.Services
{
    public class ExchangeRateService(HttpClient httpClient, ILogger<ExchangeRateService> logger) : IExchangeRateService
    {
        public async Task<List<ExchangeRate>> GetExchangeRatesAsync()
        {
            var response = await httpClient.GetAsync("http://api.nbp.pl/api/exchangerates/tables/a/last/5/");

            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException($"Failed to fetch exchange rates: {response.StatusCode}");
            }

            var content = await response.Content.ReadAsStringAsync();

            var table = JsonConvert.DeserializeObject<List<ExchangeRateTable>>(content);

            if (table == null)
            {
                var message = "No exchange rate data found.";
                logger.LogError(message, content);
                throw new InvalidOperationException(message);
            }

            return table.SelectMany(table => table.Rates)
                       .Select(rate => new ExchangeRate
                       {
                           Currency = rate.Currency,
                           Code = rate.Code,
                           Mid = rate.Mid
                       }).ToList();
        }

    }
}
