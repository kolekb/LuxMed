using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using LuxMedTest.Application.Services;
using LuxMedTest.Application.Settings;
using LuxMedTest.Domain.Interfaces;
using Moq;

namespace LuxMedTest.UnitTests.Application.Services;

public class JwtTokenServiceTests
{
    private readonly Mock<IRevokedTokenService> _revokedTokenServiceMock;
    private readonly JwtSettings _jwtSettings;
    private readonly JwtTokenService _jwtTokenService;

    public JwtTokenServiceTests()
    {
        _revokedTokenServiceMock = new Mock<IRevokedTokenService>();
        _jwtSettings = new JwtSettings
        {
            SecretKey = "SuperSecretKey12345678901234567890",
            TokenExpiryMinutes = 60,
            SlidingExpirationThresholdMinutes = 10,
            Issuer = "TestIssuer",
            Audience = "TestAudience"
        };
        _jwtTokenService = new JwtTokenService(_revokedTokenServiceMock.Object, _jwtSettings);
    }

    [Fact]
    public void WriteToken_ShouldGenerateValidToken()
    {
        // Arrange
        var username = "testUser";
        var expires = DateTime.UtcNow.AddMinutes(60);

        // Act
        var token = _jwtTokenService.WriteToken(username, expires);

        // Assert
        token.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var securityToken = handler.ReadJwtToken(token);
        securityToken.Should().NotBeNull();
        securityToken.Claims.Should().Contain(c => c.Value == username);
    }

    [Fact]
    public void ValidateToken_ShouldReturnClaimsPrincipal_WhenTokenIsValid()
    {
        // Arrange
        var username = "testUser";
        var expires = DateTime.UtcNow.AddMinutes(60);
        var token = _jwtTokenService.WriteToken(username, expires);

        // Act
        var principal = _jwtTokenService.ValidateToken(token);

        // Assert
        principal.Should().NotBeNull();
        principal!.Identity!.Name.Should().Be(username);
    }

    [Fact]
    public void ValidateToken_ShouldReturnNull_WhenTokenIsRevoked()
    {
        // Arrange
        var username = "testUser";
        var expires = DateTime.UtcNow.AddMinutes(60);
        var token = _jwtTokenService.WriteToken(username, expires);
        _revokedTokenServiceMock.Setup(r => r.IsTokenRevoked(It.IsAny<string>())).Returns(true);

        // Act
        var principal = _jwtTokenService.ValidateToken(token);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public void ShouldRenewToken_ShouldReturnTrue_WhenTokenIsCloseToExpiration()
    {
        // Arrange
        var jwtToken = new JwtSecurityToken(expires: DateTime.UtcNow.AddMinutes(9));

        // Act
        var result = _jwtTokenService.ShouldRenewToken(jwtToken);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void ShouldRenewToken_ShouldReturnFalse_WhenTokenHasSufficientTime()
    {
        // Arrange
        var jwtToken = new JwtSecurityToken(expires: DateTime.UtcNow.AddMinutes(20));

        // Act
        var result = _jwtTokenService.ShouldRenewToken(jwtToken);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void RenewToken_ShouldGenerateNewTokenWithSameClaims()
    {
        // Arrange
        var username = "testUser";
        var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.Name, username)
        }));
        var expires = DateTime.UtcNow.AddMinutes(60);

        // Act
        var token = _jwtTokenService.RenewToken(claimsPrincipal, expires);

        // Assert
        token.Should().NotBeNullOrEmpty();
        var handler = new JwtSecurityTokenHandler();
        var securityToken = handler.ReadJwtToken(token);
        securityToken.Should().NotBeNull();
        securityToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.UniqueName && c.Value == username);
    }

    [Fact]
    public void GenerateSecret_ShouldGenerateValidKey()
    {
        // Act
        var newSecret = JwtTokenService.GenerateSecret();

        // Assert
        newSecret.Length.Should().BeGreaterThan(0);
    }
}
