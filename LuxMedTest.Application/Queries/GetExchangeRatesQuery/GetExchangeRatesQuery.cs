using LuxMedTest.Application.Dtos;
using MediatR;

namespace LuxMedTest.Application.Queries.GetExchangeRatesQuery
{
    public class GetExchangeRatesQuery : IRequest<List<ExchangeRateDto>>
    {
    }
}
