using LuxMedTest.Application.Queries.GetExchangeRatesQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LuxMedTest.Api.Controllers
{
    [ApiController]
    [Route("api/exchange-rates")]
    public class ExchangeRatesController(IMediator mediator) : ControllerBase
    {
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [HttpGet]
        public async Task<IActionResult> GetExchangeRates()
        {
            var result = await mediator.Send(new GetExchangeRatesQuery());
            return Ok(result);
        }
    }
}
