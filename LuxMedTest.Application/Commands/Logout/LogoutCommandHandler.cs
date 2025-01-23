using LuxMedTest.Application.Settings;
using LuxMedTest.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace LuxMedTest.Application.Commands.Logout
{
    public class LogoutCommandHandler(IHttpContextAccessor httpContextAccessor, 
        JwtSettings jwtSettings,
        IRevokedTokenService revokedTokenService) : IRequestHandler<LogoutCommand>
    {
        Task IRequestHandler<LogoutCommand>.Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var context = httpContextAccessor.HttpContext;
            if (context?.Request.Cookies.TryGetValue(jwtSettings.CookieName, out var token) == true)
            {
                var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
                revokedTokenService.RevokeToken(jwtToken.Id);
                context.Response.Cookies.Delete(jwtSettings.CookieName);
            }

            return Task.FromResult(Unit.Value);
        }
    }
}
