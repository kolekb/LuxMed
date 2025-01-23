using FluentAssertions;
using LuxMedTest.Application.Commands.RefreshToken;
using LuxMedTest.Application.Dtos;
using LuxMedTest.Application.Settings;
using LuxMedTest.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Moq;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LuxMedTest.UnitTests.Application.Commands
{
    public class RefreshTokenCommandHandlerTests
    {
        private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
        private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
        private readonly JwtSettings _jwtSettings;
        private readonly IRequestHandler<RefreshTokenCommand, RefreshTokenDto> _handler;

        public RefreshTokenCommandHandlerTests()
        {
            _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            _jwtTokenServiceMock = new Mock<IJwtTokenService>();
            _jwtSettings = new JwtSettings
            {
                SecretKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("this_is_a_test_key_123456789")),
                TokenExpiryMinutes = 30,
                SlidingExpirationThresholdMinutes = 10,
                CookieName = "auth_cookie"
            };

            _handler = new RefreshTokenCommandHandler(
                _httpContextAccessorMock.Object,
                _jwtTokenServiceMock.Object,
                _jwtSettings
            );
        }

        [Fact]
        public async Task Handle_ShouldReturnRefreshTokenDto_WhenTokenDoesNotRequireRenewal()
        {
            // Arrange
            var oldToken = GenerateTestJwtToken(DateTime.UtcNow.AddMinutes(20));
            var claimsPrincipal = CreateClaimsPrincipal();

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(CreateHttpContextWithUser(oldToken, claimsPrincipal));
            _jwtTokenServiceMock.Setup(x => x.ShouldRenewToken(It.IsAny<JwtSecurityToken>())).Returns(false);
            _jwtTokenServiceMock.Setup(x => x.RenewToken(claimsPrincipal, It.IsAny<DateTime>()))
                .Returns("new_token");
            _jwtTokenServiceMock.Setup(x => x.ValidateToken(oldToken)).Returns(claimsPrincipal);

            // Act
            var result = await _handler.Handle(new RefreshTokenCommand(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.RefreshAfterSeconds.Should().BeLessThanOrEqualTo(20 * 60 / 2); // Połowa pozostałego czasu
        }

        [Fact]
        public async Task Handle_ShouldRenewToken_WhenTokenRequiresRenewal()
        {
            // Arrange
            var oldToken = GenerateTestJwtToken(DateTime.UtcNow.AddMinutes(5));
            var claimsPrincipal = CreateClaimsPrincipal();

            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(CreateHttpContextWithUser(oldToken, claimsPrincipal));

            _jwtTokenServiceMock.Setup(x => x.ValidateToken(oldToken)).Returns(claimsPrincipal);
            _jwtTokenServiceMock.Setup(x => x.ShouldRenewToken(It.IsAny<JwtSecurityToken>())).Returns(true);
            _jwtTokenServiceMock.Setup(x => x.RenewToken(claimsPrincipal, It.IsAny<DateTime>()))
                .Returns("new_token");

            // Act
            var result = await _handler.Handle(new RefreshTokenCommand(), CancellationToken.None);

            // Assert
            result.Should().NotBeNull();
            result.RefreshAfterSeconds.Should().Be(_jwtSettings.TokenExpiryMinutes * 60 / 2); // Połowa czasu ważności nowego tokena
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenTokenIsInvalid()
        {
            // Arrange
            var invalidToken = "invalid_token";
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(CreateHttpContext(invalidToken));

            _jwtTokenServiceMock.Setup(x => x.ValidateToken(invalidToken)).Returns((ClaimsPrincipal?)null);

            // Act
            Func<Task> act = async () => await _handler.Handle(new RefreshTokenCommand(), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("Invalid refresh token");
        }

        [Fact]
        public async Task Handle_ShouldThrowUnauthorizedAccessException_WhenNoCookieIsPresent()
        {
            // Arrange
            _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(CreateHttpContextWithoutCookie());

            // Act
            Func<Task> act = async () => await _handler.Handle(new RefreshTokenCommand(), CancellationToken.None);

            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>().WithMessage("Invalid refresh token");
        }

        private string GenerateTestJwtToken(DateTime expires)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, "testUser"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }),
                Expires = expires,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static ClaimsPrincipal CreateClaimsPrincipal()
        {
            return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "testUser"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            }, "TestAuthType"));
        }

        private static DefaultHttpContext CreateHttpContextWithUser(string token, ClaimsPrincipal user)
        {
            var context = new DefaultHttpContext();
            context.User = user;
            context.Request.Headers["Cookie"] = $"auth_cookie={token}";
            return context;
        }

        private static DefaultHttpContext CreateHttpContext(string token)
        {
            var context = new DefaultHttpContext();
            context.Request.Headers["Cookie"] = $"auth_cookie={token}";
            return context;
        }

        private static DefaultHttpContext CreateHttpContextWithoutCookie()
        {
            var context = new DefaultHttpContext();
            return context;
        }
    }
}
