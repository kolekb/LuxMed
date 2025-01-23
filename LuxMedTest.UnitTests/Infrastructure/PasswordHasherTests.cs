using FluentAssertions;
using LuxMedTest.Infrastructure.Utils;

namespace LuxMedTest.UnitTests.Infrastructure
{
    public class PasswordHasherTests
    {
        [Fact]
        public void HashPassword_ShouldGenerateDifferentHashesForSamePassword()
        {
            // Arrange
            var password = "secure_password";
            var hasher = new PasswordHasher();

            // Act
            var hash1 = hasher.HashPassword(password);
            var hash2 = hasher.HashPassword(password);

            // Assert
            hash1.Should().NotBe(hash2); // Hashy powinny być różne ze względu na różne sole
        }

        [Fact]
        public void VerifyPassword_ShouldReturnTrue_ForValidPassword()
        {
            // Arrange
            var password = "secure_password";
            var hasher = new PasswordHasher();
            var hash = hasher.HashPassword(password);

            // Act
            var result = hasher.VerifyPassword(password, hash);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void VerifyPassword_ShouldReturnFalse_ForInvalidPassword()
        {
            // Arrange
            var password = "secure_password";
            var wrongPassword = "wrong_password";
            var hasher = new PasswordHasher();
            var hash = hasher.HashPassword(password);

            // Act
            var result = hasher.VerifyPassword(wrongPassword, hash);

            // Assert
            result.Should().BeFalse();
        }

    }
}
