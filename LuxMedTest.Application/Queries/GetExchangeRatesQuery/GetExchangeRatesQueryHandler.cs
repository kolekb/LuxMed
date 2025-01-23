using AutoMapper;
using LuxMedTest.Application.Dtos;
using LuxMedTest.Domain.Interfaces;
using MediatR;

namespace LuxMedTest.Application.Queries.GetExchangeRatesQuery
{
    public class GetExchangeRatesQueryHandler(IExchangeRateService exchangeRateService, IMapper mapper) : IRequestHandler<GetExchangeRatesQuery, List<ExchangeRateDto>>
    {
        public async Task<List<ExchangeRateDto>> Handle(GetExchangeRatesQuery request, CancellationToken cancellationToken)
        {
            var exchangeRates = await exchangeRateService.GetExchangeRatesAsync();

            var response = mapper.Map<List<ExchangeRateDto>>(exchangeRates);

            return response;
        }
    }
}
