using LuxMedTest.Application.Dtos;
using LuxMedTest.Application.Settings;
using LuxMedTest.Application.Utils;
using LuxMedTest.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;

namespace LuxMedTest.Application.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler(IHttpContextAccessor httpContextAccessor, 
        IJwtTokenService jwtTokenService,
        JwtSettings jwtSettings) : IRequestHandler<RefreshTokenCommand, RefreshTokenDto>
    {
        Task<RefreshTokenDto> IRequestHandler<RefreshTokenCommand, RefreshTokenDto>.Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var context = httpContextAccessor.HttpContext;
            if (context.User.Identity?.IsAuthenticated == true)
            {
                if(context.Request.Cookies.TryGetValue(jwtSettings.CookieName, out var oldToken))
                {
                    var tokenExpiresInSeconds = jwtSettings.TokenExpiryMinutes * 60;

                    // Sprawdź, czy token wymaga odnowienia
                    var jwtToken = new JwtSecurityTokenHandler().ReadJwtToken(oldToken);
                    var timeToExpiration = jwtToken.ValidTo - DateTime.UtcNow;

                    if (!jwtTokenService.ShouldRenewToken(jwtToken))
                    {
                        var safeRefreshTimeInSeconds = (int)(timeToExpiration.TotalSeconds / 2); // Połowa pozostałego czasu

                        return Task.FromResult(new RefreshTokenDto
                        {
                            RefreshAfterSeconds = safeRefreshTimeInSeconds,
                        });
                    }

                    var claimsPrincipal = jwtTokenService.ValidateToken(oldToken);
                    if (claimsPrincipal != null) 
                    {
                        var expires = DateTime.UtcNow.AddSeconds(tokenExpiresInSeconds);
                        var token = jwtTokenService.RenewToken(claimsPrincipal, expires);

                        CookieHelper.SetTokenCookie(httpContextAccessor.HttpContext?.Response!, jwtSettings.CookieName, token, expires);

                        return Task.FromResult(new RefreshTokenDto
                        {
                            RefreshAfterSeconds = tokenExpiresInSeconds / 2,
                        });
                    }
                }
            }

            throw new UnauthorizedAccessException("Invalid refresh token");
        }
    }
}
