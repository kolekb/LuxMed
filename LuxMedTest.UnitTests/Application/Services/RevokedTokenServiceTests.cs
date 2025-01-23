using FluentAssertions;
using LuxMedTest.Application.Services;

namespace LuxMedTest.UnitTests.Application.Services
{
    public class RevokedTokenServiceTests
    {
        [Fact]
        public void RevokeToken_ShouldAddTokenIdToRevokedList()
        {
            // Arrange
            var service = new RevokedTokenService();
            var tokenId = Guid.NewGuid().ToString();

            // Act
            service.RevokeToken(tokenId);

            // Assert
            service.IsTokenRevoked(tokenId).Should().BeTrue();
        }

        [Fact]
        public void IsTokenRevoked_ShouldReturnFalse_WhenTokenIsNotRevoked()
        {
            // Arrange
            var service = new RevokedTokenService();
            var tokenId = Guid.NewGuid().ToString();

            // Act
            var isRevoked = service.IsTokenRevoked(tokenId);

            // Assert
            isRevoked.Should().BeFalse();
        }

        [Fact]
        public void RevokeToken_ShouldNotThrowException_WhenRevokingSameTokenMultipleTimes()
        {
            // Arrange
            var service = new RevokedTokenService();
            var tokenId = Guid.NewGuid().ToString();

            // Act
            Action act = () =>
            {
                service.RevokeToken(tokenId);
                service.RevokeToken(tokenId);
            };

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void IsTokenRevoked_ShouldHandleEmptyTokenId()
        {
            // Arrange
            var service = new RevokedTokenService();
            var tokenId = string.Empty;

            // Act
            var isRevoked = service.IsTokenRevoked(tokenId);

            // Assert
            isRevoked.Should().BeFalse();
        }

        [Fact]
        public void IsTokenRevoked_ShouldBeCaseSensitive()
        {
            // Arrange
            var service = new RevokedTokenService();
            var tokenId = Guid.NewGuid().ToString();
            var tokenIdUpperCase = tokenId.ToUpper();

            // Act
            service.RevokeToken(tokenId);
            var isRevokedOriginal = service.IsTokenRevoked(tokenId);
            var isRevokedUpperCase = service.IsTokenRevoked(tokenIdUpperCase);

            // Assert
            isRevokedOriginal.Should().BeTrue("because the original token was revoked");
            isRevokedUpperCase.Should().BeFalse("because token IDs are case-sensitive");
        }
    }
}
