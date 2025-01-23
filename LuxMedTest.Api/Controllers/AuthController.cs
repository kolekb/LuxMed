using LuxMedTest.Application.Commands.Login;
using LuxMedTest.Application.Commands.Logout;
using LuxMedTest.Application.Commands.RefreshToken;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LuxMedTest.Api.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController(IMediator mediator) : ControllerBase
    {
        [HttpPost("refresh-token")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] RefreshTokenCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var result = await mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("logout")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
        {
            await mediator.Send(command);
            return Ok();
        }
    }
}
