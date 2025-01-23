using LuxMedTest.Domain.Models;

namespace LuxMedTest.Domain.Interfaces
{
    public interface IExchangeRateService
    {
        Task<List<ExchangeRate>> GetExchangeRatesAsync();
    }
}
