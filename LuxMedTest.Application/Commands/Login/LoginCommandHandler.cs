using LuxMedTest.Application.Dtos;
using LuxMedTest.Application.Settings;
using LuxMedTest.Application.Utils;
using LuxMedTest.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace LuxMedTest.Application.Commands.Login
{
    public class LoginCommandHandler(IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IHttpContextAccessor httpContextAccessor,
        JwtSettings jwtSettings) : IRequestHandler<LoginCommand, RefreshTokenDto>
    {
        async Task<RefreshTokenDto> IRequestHandler<LoginCommand, RefreshTokenDto>.Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await userRepository.GetByUsernameAsync(request.Username);
            if (user == null || !passwordHasher.VerifyPassword(request.Password, user.Password))
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            SetCookie(user.Username, out var tokenExpiresInSeconds);

            return new RefreshTokenDto
            {
                RefreshAfterSeconds = tokenExpiresInSeconds / 2
            };
        }

        private void SetCookie(string username, out int tokenExpiresInSeconds)
        {
            tokenExpiresInSeconds = jwtSettings.TokenExpiryMinutes * 60;
            var expires = DateTime.UtcNow.AddSeconds(tokenExpiresInSeconds);
            var token = jwtTokenService.WriteToken(username, expires);

            CookieHelper.SetTokenCookie(httpContextAccessor.HttpContext?.Response!, jwtSettings.CookieName, token, expires);
        }

    }
}
