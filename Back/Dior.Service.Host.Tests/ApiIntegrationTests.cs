using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using Dior.Library.DTO.Auth;
using Dior.Library.DTO.User;
using Microsoft.AspNetCore.Hosting;

namespace Dior.Service.Host.Tests
{
    public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ApiIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Get_Status_ReturnsSuccess()
        {
            // Arrange & Act
            var response = await _client.GetAsync("/api/status");

            // Assert
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.Contains("API Dior fonctionne correctement", content);
        }

        [Fact]
        public async Task Post_Login_WithValidCredentials_ReturnsToken()
        {
            // Arrange
            var loginRequest = new LoginRequestDto
            {
                Username = "admin",
                Password = "admin"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

            // Assert
            response.EnsureSuccessStatusCode();
            var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseCompleteDto>();
            Assert.NotNull(loginResponse);
            Assert.NotEmpty(loginResponse.Token);
        }

        [Fact]
        public async Task Get_Users_ReturnsUserList()
        {
            // Act
            var response = await _client.GetAsync("/api/user");

            // Assert
            response.EnsureSuccessStatusCode();
            var users = await response.Content.ReadFromJsonAsync<List<UserDto>>();
            Assert.NotNull(users);
        }

        [Fact]
        public async Task Get_Teams_ReturnsTeamList()
        {
            // Act
            var response = await _client.GetAsync("/api/team");

            // Assert
            response.EnsureSuccessStatusCode();
            var teams = await response.Content.ReadFromJsonAsync<List<TeamDto>>();
            Assert.NotNull(teams);
        }

        [Fact]
        public async Task Get_Swagger_ReturnsSuccess()
        {
            // Act
            var response = await _client.GetAsync("/swagger/v1/swagger.json");

            // Assert
            response.EnsureSuccessStatusCode();
        }
    }
}