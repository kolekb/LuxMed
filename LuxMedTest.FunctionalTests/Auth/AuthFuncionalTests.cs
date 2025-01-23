using System.Text;
using System.Text.Json;
using FluentAssertions;
using LuxMedTest.FunctionalTests.Fixtures;

namespace LuxMedTest.FunctionalTests.Auth
{
    [Collection("DbTests")]
    public class AuthFuncionalTests(TestFixture fixture)
    {
        private readonly HttpClient _client = fixture.Factory.CreateClient();

        [Fact]
        public async Task Login_ShouldReturnSuccess_WhenCredentialsAreValid()
        {
            // Arrange
            var content = new StringContent(
                JsonSerializer.Serialize(new { Username = "admin", Password = "hashed_admin123" }),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            response.Headers.Contains("Set-Cookie").Should().BeTrue();
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenCredentialsAreInvalid()
        {
            // Arrange
            var content = new StringContent(
                JsonSerializer.Serialize(new { Username = "admin", Password = "wrongpassword" }),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenPayloadIsEmpty()
        {
            // Arrange
            var content = new StringContent("", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenPayloadIsMalformed()
        {
            // Arrange
            var content = new StringContent("{ invalid_json }", Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenFieldsAreMissing()
        {
            // Arrange
            var content = new StringContent(
                JsonSerializer.Serialize(new { Username = "admin" }),
                Encoding.UTF8,
                "application/json"
            );

            // Act
            var response = await _client.PostAsync("/api/auth/login", content);

            // Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
